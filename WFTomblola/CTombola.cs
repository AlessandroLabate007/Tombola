using System;
using System.Collections.Generic;

namespace WFTomblola;

public class CTombola
{
    private int[,] _scheda = new int[3, 9];
    private bool[,] _segnati = new bool[3, 9];
    private int _numero;
    public static int[] verifica = new int[91];
    private int[] _posizioni = new int[9]; 
    private static int _click;
    public static bool ambo, terna, quaterna, cinquina = false;
    

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
            if (_numero >= 1 && _numero <= 90)
            {
                if (verifica[_numero] == 0)
                {
                    verifica[_numero] = _numero;
                    _click++;
                }
            }
        }
    }

    public void GeneraNuovaScheda()
    {
        // 1. Reset della matrice
        Array.Clear(_scheda, 0, _scheda.Length);

        // 2. Distribuzione dei 15 numeri nelle 9 colonne 
        // Per avere 5 numeri per riga (5*3=15) con 9 colonne:
        // Servono 4 colonne con 1 numero, 4 con 2 numeri e 1 con 3 numeri.
        
        int[] colTypes = { 1, 1, 1, 1, 2, 2, 2, 2, 3 };
        // Mischiamo i tipi di colonna
        for (int i = 0; i < 9; i++)
        {
            int swapIdx = Random.Shared.Next(i, 9);
            (colTypes[i], colTypes[swapIdx]) = (colTypes[swapIdx], colTypes[i]);
        }

        // Distribuzione righe per le colonne con 2 numeri
        // Vogliamo bilanciare: 2x (0,1), 1x (0,2), 1x (1,2)
        int[][] pairPatterns = { new[] { 0, 1 }, new[] { 0, 1 }, new[] { 0, 2 }, new[] { 1, 2 } };
        int pairIdx = 0;

        // Distribuzione righe per le colonne con 1 numero
        // Vogliamo bilanciare: 1x (0), 1x (1), 2x (2)
        int[] singlePatterns = { 0, 1, 2, 2 };
        int singleIdx = 0;

        // 3. Generazione numeri e posizionamento
        for (int col = 0; col < 9; col++)
        {
            int quantiInColonna = colTypes[col];
            List<int> numeri = GeneraNumeriPerColonna(col, quantiInColonna);

            if (quantiInColonna == 3)
            {
                _scheda[0, col] = numeri[0];
                _scheda[1, col] = numeri[1];
                _scheda[2, col] = numeri[2];
            }
            else if (quantiInColonna == 2)
            {
                int[] rows = pairPatterns[pairIdx++];
                _scheda[rows[0], col] = numeri[0];
                _scheda[rows[1], col] = numeri[1];
            }
            else if (quantiInColonna == 1)
            {
                int row = singlePatterns[singleIdx++];
                _scheda[row, col] = numeri[0];
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
        do { nuovo = Random.Shared.Next(1, 91); } while (verifica[nuovo] != 0);
        verifica[nuovo] = nuovo;
        _numero = nuovo;
        _click++;
    }
    
    
}
