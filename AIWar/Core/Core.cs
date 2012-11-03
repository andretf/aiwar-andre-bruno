using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIWar.Core
{
    static public class Core
    {
        static private int[] excludeCasas = {0, 1, 3, 4, 5, 6, 9, 10, 11, 15, 16, 21, 93, 98, 99, 103, 104, 105, 108, 109, 110, 111, 113, 114};
        static private int[] Boundary = { 2, 7, 8, 12, 14, 17, 20, 22, 26, 27, 32, 38, 43, 49, 54, 60, 65, 71, 76, 82, 87, 88, 92, 94, 97, 100, 102, 106, 107, 112 };

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

                if (Boundary.Contains(ii))
                    break;
            }

            for(int ii = i+step; ii < 115; ii += step){
                if (TabuleiroVetor[ii] > 0 || excludeCasas.Contains(ii))
                    break;
                
                result.Add(ii);
                
                if (Boundary.Contains(ii))
                    break;
            }
            
            return result.ToArray();
        }

        /// <summary>
        /// subtrai ou adiciona 11 (5 + 6) ao índice de linha.
        /// </summary>
        /// <returns></returns>
        static public int[] getVisaoVertical(int[] TabuleiroVetor, int i) {
            return AddCasa(TabuleiroVetor, i, 11);
        }

        /// <summary>
        /// Diagonal Menor: subtrai ou adiciona 5 ao índice da linha.
        /// </summary>
        /// <returns></returns>
        static public int[] getVisaoDiagonalMenor(int[] TabuleiroVetor, int i)
        {
            return AddCasa(TabuleiroVetor, i, 5);
        }

        /// <summary>
        /// Diagonal Maior: subtrai ou adiciona 6 ao índice da linha.
        /// </summary>
        /// <returns></returns>
        static public int[] getVisaoDiagonalMaior(int[] TabuleiroVetor, int i)
        {
            return AddCasa(TabuleiroVetor, i, 6);
        }
    }
}
