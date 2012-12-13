
namespace AIWar.Core {
    public class Nodo {
        public int[] estadoTabuleiro;
        public int CapturaBalance;
        public int MoveDe;
        public int MovePara;
        public Nodo[] filhos;

        public Nodo(int[] tabuleiroAnterior) {
            estadoTabuleiro = new int[137];
            tabuleiroAnterior.CopyTo(estadoTabuleiro, 0);
            filhos = new Nodo[] { };
            CapturaBalance = 0;
        }

        public Nodo(int[] tabuleiroAnterior, int peca, int from, int to) {
            filhos = new Nodo[]{};
            CapturaBalance = 0;
            
            estadoTabuleiro = new int[137];
            tabuleiroAnterior.CopyTo(estadoTabuleiro, 0);

            estadoTabuleiro[from] = 0;
            estadoTabuleiro[to] = peca;
            
            // se desencadear capturas do rival, -- o Saldo
            Enums.pColor cor = getPecaCor(from);
            foreach (int i in Core.getPecasPosicao(estadoTabuleiro, cor)) {
                if (PecaCapturavel(i, cor)) {
                    if(i == to) estadoTabuleiro[to] = 0;
                    CapturaBalance--;
                }
            }

            // se desencadear capturas próprias, ++ o Saldo
            switch (cor) {
                case Enums.pColor.preta:    cor = Enums.pColor.branca;  break;
                case Enums.pColor.branca:   cor = Enums.pColor.preta;   break; 
            }
            foreach (int i in Core.getPecasPosicao(estadoTabuleiro, cor)) {
                if (PecaCapturavel(i, cor)) {
                    CapturaBalance++;
                }
            }
        
            MoveDe = from;
            MovePara = estadoTabuleiro[to];
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
