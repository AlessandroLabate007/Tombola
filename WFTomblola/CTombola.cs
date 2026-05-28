using System;
using System.Collections.Generic;

namespace WFTomblola;

public class CTombola
{
    private int[,] _scheda = new int[3, 9];
    private bool[,] _segnati = new bool[3, 9];
    private int _numero;
    private static int[] _verifica = new int[91];
    private int[] _posizioni = new int[9]; 
    private static int _click;
    public bool ambo, terna, quaterna, cinquina = false;

    public CTombola()
    {
        _numero = 0;
        GeneraNuovaScheda();
    }
    
    public int[,] scheda => _scheda;

    public void Segna(int r, int c, bool stato)
    {
        _segnati[r, c] = stato;
    }

    public string ControllaVincita(int r)
    {
        int cont = 0;
        for (int c = 0; c < 9; c++)
        {
            if (_segnati[r, c]) cont++;
        }
        
        if (cont == 2 && ambo == false)
        {
            ambo = true;
            return "AMBO";
        }
        else if (cont == 3 && terna == false)
        {
            terna = true;
            return "TERNA";
        }
        else if (cont == 4 && quaterna == false)
        {
            quaterna = true;
            return "QUATERNA";
        }
        else if (cont == 5 && cinquina == false)
        {
            cinquina = true;
            return "CINQUINA";
        }

        return "";
    }

    public int numero
    {
        get { return _numero; }
        set
        {
            _numero = value;
            GeneraNumero();
        }
    }

    public void GeneraNuovaScheda()
    {
        // 1. Reset della matrice
        Array.Clear(_scheda, 0, _scheda.Length);

        // 2. Distribuzione dei 15 numeri nelle 9 colonne (Regola: min 1, max 3 per colonna)
        for (int i = 0; i < 9; i++) _posizioni[i] = 1; 
        int extra = 6; 
        while (extra > 0)
        {
            int c = Random.Shared.Next(0, 9);
            if (_posizioni[c] < 3) { _posizioni[c]++; extra--; }
        }

        // 3. Generazione numeri e posizionamento intelligente
        for (int col = 0; col < 9; col++)
        {
            int quantiInColonna = _posizioni[col];
            List<int> numeri = GeneraNumeriPerColonna(col, quantiInColonna);

            // Applichiamo le regole di posizionamento verticale
            if (quantiInColonna == 3)
            {
                // Tre numeri: tutti in colonna (righe 0, 1, 2)
                _scheda[0, col] = numeri[0];
                _scheda[1, col] = numeri[1];
                _scheda[2, col] = numeri[2];
            }
            else if (quantiInColonna == 2)
            {
                // Due numeri: divisi da uno spazio (righe 0 e 2)
                _scheda[0, col] = numeri[0];
                _scheda[1, col] = 0; // Spazio vuoto
                _scheda[2, col] = numeri[1];
            }
            else if (quantiInColonna == 1)
            {
                // Un numero: solo al centro (riga 1)
                _scheda[0, col] = 0;
                _scheda[1, col] = numeri[0];
                _scheda[2, col] = 0;
            }
        }
    }

    private List<int> GeneraNumeriPerColonna(int col, int quanti)
    {
        List<int> numeri = new List<int>();
        int min = (col == 0) ? 1 : col * 10;
        int max = (col == 8) ? 91 : (col + 1) * 10;

        while (numeri.Count < quanti)
        {
            int n = Random.Shared.Next(min, max);
            if (!numeri.Contains(n)) numeri.Add(n);
        }
        numeri.Sort();
        return numeri;
    }

    public void GeneraNumero()
    {
        if (_click >= 90) { _numero = -1; return; }
        int nuovo;
        do { nuovo = Random.Shared.Next(1, 91); } while (_verifica[nuovo] != 0);
        _verifica[nuovo] = nuovo;
        _numero = nuovo;
        _click++;
    }
}
