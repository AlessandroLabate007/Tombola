using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Threading.Timer;

namespace WFTomblola;

public partial class Form1 : Form
{
    CTombola gioco = new CTombola();
    

    public Form1()
    {
        InitializeComponent();
        InizializzaTabellaDinamica();
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
            if (gioco.numero == Int32.Parse(lbl.Text))
            {
                var pos = tableLayoutPanel1.GetCellPosition(lbl);
                int r = pos.Row;
                int c = pos.Column;

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
                        await Task.Delay(1500);
                        label32.Text = "";
                    }
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

    private void button1_Click(object sender, EventArgs e)
    {
        gioco.GeneraNumero();
        
        if (gioco.numero == -1)
        {
            label3.Text = "Fine";
        }
        else
        {
            label3.Text = gioco.numero.ToString();
        }
    }

    private void label5_Click(object sender, EventArgs e)
    {
        
    }
    
    
}
