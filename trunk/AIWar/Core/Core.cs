using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AIWar.Core
{
    class Core
    {
        private int[] excludeCasas = {0, 1, 3, 4, 5, 6, 9, 10, 11, 15, 16, 21, 93, 98, 99, 103, 104, 105, 108, 109, 110, 111, 113, 114};


        /// <summary>
        /// Função auxiliar.
        /// Adiciona casa a aquelas que estão no raio de comilança da atual;.
        /// </summary>
        /// <param name="TabuleiroVetor"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        private int[] AddCasa(int[] TabuleiroVetor, int i, int step)
        {
            List<int> result = new List<int>();
            
            for(int ii = i; ii >= 0; ii -= step){
                if (TabuleiroVetor[ii] > 0 || excludeCasas.Contains(ii))
                    break;
                result.Add(ii);
            }

            for(int ii = i; ii < 115; ii += step){
                if (TabuleiroVetor[ii] > 0 || excludeCasas.Contains(ii))
                    break;
                result.Add(ii);
            }
            
            return result.ToArray();
        }

        /// <summary>
        /// subtrai ou adiciona 11 (5 + 6) ao índice de linha.
        /// </summary>
        /// <returns></returns>
        int[] getVisaoVertical(int[] TabuleiroVetor, int i) {
            return AddCasa(TabuleiroVetor, i, 11);
        }

        /// <summary>
        /// Diagonal Menor: subtrai ou adiciona 5 ao índice da linha.
        /// </summary>
        /// <returns></returns>
        int[] getVisaoDiagonalMenor(int[] TabuleiroVetor, int i) {
            return AddCasa(TabuleiroVetor, i, 5);
        }

        /// <summary>
        /// Diagonal Maior: subtrai ou adiciona 5 ao índice da linha.
        /// </summary>
        /// <returns></returns>
        int[] getVisaoDiagonalMaior(int[] TabuleiroVetor, int i)
        {
            return AddCasa(TabuleiroVetor, i, 6);
        }
    }
}
