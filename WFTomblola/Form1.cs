using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace WFTomblola;

public partial class Form1 : Form
{
    CTombola gioco = new CTombola();
    MqttManager _net;
    string _nomeGiocatore = "Giocatore_" + Guid.NewGuid().ToString().Substring(0, 5);
    int vinto = 0;
    int numero;

    public Form1()
    {
        InitializeComponent();
        InizializzaTabellaDinamica();
        ConfiguraRete();
        label33.Click += label33_Click;
        label33.Cursor = Cursors.Hand;
    }

    private void ConfiguraRete()
    {
        // Broker pubblico per permettere il gioco tra Italia e America
        _net = new MqttManager("broker.emqx.io", _nomeGiocatore);


        _net.OnNumeroRicevuto += (num) =>
        {
            this.Invoke((MethodInvoker)delegate {
                gioco.numero = num;
                label3.Text = num.ToString();
            });
        };

        _net.OnMessaggioRicevuto += (msg) =>
        {
            this.Invoke((MethodInvoker)delegate {
                label32.Text = msg;
                Task.Delay(3000).ContinueWith(_ => {
                    this.Invoke((MethodInvoker)delegate { label32.Text = ""; });
                });
            });
        };
    }

    private void InizializzaTabellaDinamica()
    {
        tableLayoutPanel1.Controls.Clear();

        for (int r = 0; r < 3; r++)
        {
            for (int c = 0; c < 9; c++)
            {
                Label lbl = new Label();
                lbl.Dock = DockStyle.Fill;
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.Font = new Font("Arial", 11, FontStyle.Bold);
                lbl.BorderStyle = BorderStyle.FixedSingle;
                lbl.Margin = new Padding(1);

                int valore = gioco.scheda[r, c];
                lbl.Text = (valore == 0) ? "" : valore.ToString();

                // AGGIUNTO: Colleghiamo l'evento Click a ogni label
                // Solo se la label non è vuota
                if (valore != 0)
                {
                    lbl.Cursor = Cursors.Hand; // Cambia il cursore per far capire che è cliccabile
                    lbl.Click += LabelSchedina_Click;
                }

                tableLayoutPanel1.Controls.Add(lbl, c, r);
            }
        }
    }

    /// <summary>
    /// Evento che scatta quando clicchi su un numero della schedina.
    /// </summary>
    private async void LabelSchedina_Click(object sender, EventArgs e)
    {
        if (sender is Label lbl)
        {
            
            if (vinto == 14)
                label32.Text = "HAI VINTO!";
            
            if (gioco.numero == Int32.Parse(lbl.Text) && vinto < 15)
            {
                var pos = tableLayoutPanel1.GetCellPosition(lbl);
                int r = pos.Row;
                int c = pos.Column;
                vinto++;

                if (lbl.BackColor == Color.OrangeRed)
                {
                    // Se è già segnato, lo riportiamo al colore normale
                    lbl.BackColor = SystemColors.Control;
                    lbl.ForeColor = Color.Black;
                    gioco.Segna(r, c, false);
                }
                else
                {
                    // Lo segnamo con un colore acceso
                    lbl.BackColor = Color.OrangeRed;
                    lbl.ForeColor = Color.White;
                    gioco.Segna(r, c, true);

                    string vincita = gioco.ControllaVincita(r);
                    if (!string.IsNullOrEmpty(vincita))
                    {
                        label32.Text = vincita;
                        _net.InviaVincita(_nomeGiocatore, vincita); // Invia a tutti!
                        await Task.Delay(1500);
                        label32.Text = "";
                    }
                }
            }
            
            else
            {
                // Se il numero è stato già estratto, permetti di segnarlo (recupero)
                int valoreLabel = Int32.Parse(lbl.Text);
                if (CTombola.verifica[valoreLabel] != 0)
                {
                    var pos = tableLayoutPanel1.GetCellPosition(lbl);
                    lbl.BackColor = Color.OrangeRed;
                    lbl.ForeColor = Color.White;
                    gioco.Segna(pos.Row, pos.Column, true);
                    
                    string vincita = gioco.ControllaVincita(pos.Row);
                    if (!string.IsNullOrEmpty(vincita))
                    {
                        label32.Text = vincita;
                        _net.InviaVincita(_nomeGiocatore, vincita);
                        await Task.Delay(1500);
                        label32.Text = "";
                    }
                }
                else
                {
                    label32.Text = "NON BARARE!";
                    await Task.Delay(1500);
                    label32.Text = "";
                }
            }
        }
    }

    private async void button1_Click(object sender, EventArgs e)
    {
        gioco.GeneraNumero();

        if (gioco.numero == -1)
        {
            label3.Text = "Fine";
        }
        else
        {
            label3.Text = gioco.numero.ToString();
            await _net.InviaNumero(gioco.numero); // Invia il numero agli altri
        }
    }

    private async void label33_Click(object sender, EventArgs e)
    {
        try 
        {
            label33.Text = "IN CORSO...";
            label33.ForeColor = Color.Orange;
            await _net.Connetti();
            label33.Text = "CONNESSO";
            label33.ForeColor = Color.Green;
        }
        catch (Exception ex)
        {
            label33.Text = "ERRORE";
            label33.ForeColor = Color.Red;
            MessageBox.Show("Errore connessione: " + ex.Message + "\n\nDettagli: " + ex.InnerException?.Message);
        }
    }


    private void label5_Click(object sender, EventArgs e)
    {
        
    }
}
