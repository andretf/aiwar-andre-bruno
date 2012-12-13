
namespace AIWar.Core {
    public class Nodo {
        public int[] estadoTabuleiro;
        public int sumCapturadas;
        public Nodo[] filhos;

        public Nodo(int[] tabuleiroAnterior) {
            estadoTabuleiro = new int[137];
            tabuleiroAnterior.CopyTo(estadoTabuleiro, 0);
            filhos = new Nodo[] { };
            sumCapturadas = 0;
        }

        public Nodo(int[] tabuleiroAnterior, int peca, int from, int to, int capturadas) {
            filhos = new Nodo[]{};
            sumCapturadas = capturadas;
            
            estadoTabuleiro = new int[137];
            tabuleiroAnterior.CopyTo(estadoTabuleiro, 0);

            estadoTabuleiro[from] = 0;
            estadoTabuleiro[to] = peca;

            // se for catpurável, marca como capturada
            if(PecaCapturavel(to, getPecaCor(to))){
                estadoTabuleiro[to] = 0;
                sumCapturadas++;
            }
        }

        private bool PecaCapturavel(int pos, Enums.pColor cor){
            int qtd = 0;
            int cargasSoma = 0;

            foreach (int i in Core.getPecasVisiveis(estadoTabuleiro, pos))
            {
                cargasSoma += getParticulaCarga(i);
                if (getPecaCor(i) == Enums.pColor.preta && cor == Enums.pColor.branca)
                    qtd++;
                else if (getPecaCor(i) == Enums.pColor.branca && cor == Enums.pColor.preta)
                    qtd++;
            }

            return (qtd > 1) && (cargasSoma == 0);
        }

        private Enums.pColor getPecaCor(int i) {
            if (estadoTabuleiro[i] > 0 && estadoTabuleiro[i] < 4)
                return Enums.pColor.preta;
            else if(estadoTabuleiro[i] > 3)
                return Enums.pColor.branca;
            return Enums.pColor.undefined;
        }

        private int getParticulaCarga(int i) {
            switch (estadoTabuleiro[i]) {
                case 2: case 5: return -1;
                case 3: case 6: return 1;
            }
            return 0;
        }

    }
}
