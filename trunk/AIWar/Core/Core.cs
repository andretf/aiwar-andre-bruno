using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIWar.Core
{
    static public class Core
    {
        const int MAX_VETOR = 137;
        static public int[] excludeCasas = { 0, 1, 2, 4, 5, 6, 7, 8, 11, 12, 13, 14, 18, 19, 20, 25, 26, 32, 39, 45, 52, 58, 65, 71, 78, 84, 91, 97,
                                           104, 110, 111, 116, 117, 118, 122, 123, 124, 125, 128, 129, 130, 131, 132, 134, 135, 136 };

        #region "Casas Visíveis"
        /// <summary>
        /// Função auxiliar.
        /// Adiciona casa a aquelas que estão no raio de comilança da atual;.
        /// </summary>
        /// <param name="TabuleiroVetor"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        static private int[] AddCasa(int[] TabuleiroVetor, int i, int step)
        {
            List<int> result = new List<int>();
            
            for(int ii = i-step; ii >= 0; ii -= step){
                if (TabuleiroVetor[ii] > 0 || excludeCasas.Contains(ii))
                    break;
                
                result.Add(ii);
            }

            for(int ii = i+step; ii < MAX_VETOR; ii += step){
                if (TabuleiroVetor[ii] > 0 || excludeCasas.Contains(ii))
                    break;
                
                result.Add(ii);
            }
            
            return result.ToArray();
        }

        /// <summary>
        /// subtrai ou adiciona 13 (7 + 6) ao índice de linha.
        /// </summary>
        /// <returns></returns>
        static private int[] getVisaoVertical(int[] TabuleiroVetor, int i) {
            return AddCasa(TabuleiroVetor, i, 13);
        }

        /// <summary>
        /// Diagonal Menor: subtrai ou adiciona 6 ao índice da linha.
        /// </summary>
        /// <returns></returns>
        static private int[] getVisaoDiagonalMenor(int[] TabuleiroVetor, int i)
        {
            return AddCasa(TabuleiroVetor, i, 6);
        }

        /// <summary>
        /// Diagonal Maior: subtrai ou adiciona 7 ao índice da linha.
        /// </summary>
        /// <returns></returns>
        static private int[] getVisaoDiagonalMaior(int[] TabuleiroVetor, int i)
        {
            return AddCasa(TabuleiroVetor, i, 7);
        }

        static public int[] getCasasVisiveis(int[] TabuleiroVetor, int i) {
            List<int> result = new List<int>();

            result.AddRange(getVisaoVertical(TabuleiroVetor, i));
            result.AddRange(getVisaoDiagonalMenor(TabuleiroVetor, i));
            result.AddRange(getVisaoDiagonalMaior(TabuleiroVetor, i));

            return result.ToArray();
        }
        #endregion

        #region "Peças Visíveis"
        /// <summary>
        /// Função auxiliar.
        /// Adiciona casa a aquelas que estão no raio de comilança da atual;.
        /// </summary>
        /// <param name="TabuleiroVetor"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        static private int[] AddPeca(int[] TabuleiroVetor, int i, int step) {
            List<int> result = new List<int>();

            for (int ii = i - step; ii >= 0; ii -= step) {
                if (excludeCasas.Contains(ii))
                    break;

                if (TabuleiroVetor[ii] > 0) {
                    result.Add(ii);
                    break;
                }
            }

            for (int ii = i + step; ii < MAX_VETOR; ii += step) {
                if (excludeCasas.Contains(ii))
                    break;

                if (TabuleiroVetor[ii] > 0) {
                    result.Add(ii);
                    break;
                }
            }

            return result.ToArray();
        }
        
        /// <summary>
        /// Retorna o índice de cada uma das peças que estão na linha de visão.
        /// </summary>
        /// <param name="TabuleiroVetor">Tabuleiro.</param>
        /// <param name="i">Índice da peça em questão.</param>
        /// <returns></returns>
        static public int[] getPecasVisiveis(int[] TabuleiroVetor, int i) {
            List<int> result = new List<int>();

            if (!excludeCasas.Contains(i)) {
                result.AddRange(AddPeca(TabuleiroVetor, i, 13));
                result.AddRange(AddPeca(TabuleiroVetor, i, 7));
                result.AddRange(AddPeca(TabuleiroVetor, i, 6));
            }

            return result.ToArray();
        }
        #endregion

        static public Enums.pColor getPecaCor(int i) {
            switch (i) {
                case 1:
                case 2:
                case 3:
                    return Enums.pColor.preta;
                case 4:
                case 5:
                case 6:
                    return Enums.pColor.branca;
            }
            return Enums.pColor.undefined;
        }

        static public Nodo[] addNodosArvore(Nodo ArvoreNodo, Enums.pColor cor) {
            List<Nodo> result = new List<Nodo>();

            for (int i = 0; i < 137; i++){
                // Se é uma peça da mesma cor
                if (getPecaCor(ArvoreNodo.estadoTabuleiro[i]) == cor) {
                    // e ela tem pra onde ir
                    // então cria o nodo pra cada uma das possiveis jogadas
                    foreach(int casa in getCasasVisiveis(ArvoreNodo.estadoTabuleiro, i)) {
                        result.Add(new Nodo(ArvoreNodo.estadoTabuleiro,
                                            ArvoreNodo.estadoTabuleiro[i],
                                            i,
                                            casa,
                                            ArvoreNodo.sumCapturadas));
                    }
                }
            }

            return result.ToArray();
        }

        //        ROTINA negamax(nó, profundidade)
        //    SE nó é um nó terminal OU profundidade = 0 ENTÃO
        //        RETORNE o valor da heurística do nó
        //    SENÃO
        //        alfa ← -∞                       { a avaliação é idêntica para ambos os jogadores }
        //        PARA CADA filho DE nó
        //            alfa ← max(alfa, -negamax(filho, profundidade-1))
        //        FIM PARA
        //        RETORNE alfa
        //    FIM SE
        //FIM ROTINA

        static public int Negamax(Nodo node, int depth, int alfa, int beta){
            if(node.filhos.Length == 0 || depth == 0){
                return node.sumCapturadas;
            }
            else{
        //        foreach child in node.Childs{
        //            val = -negamax(child, depth-1, -beta, -alfa, -color);

        //            // Alfa-beta corte
        //            if (val >= beta)
        //                return val;
        //            if (val >= alfa)
        //                alfa = val;
        //        }
        //        return alfa;
            }
            return 0;
        }
    }
}
