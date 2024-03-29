﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AIWar.Properties;
using AIWar.Core;

namespace AIWar
{
    // TODO:
    // - PC jogador: entre as casas visiveis, escolher uma!
    // - Minimax
    public partial class Form1 : Form
	{
		#region "Properties, Globals"
        // 1 - Neutron Preto
        // 2 - Eletron Preto
        // 3 - Proton Preto
        // 4 - Neutron Branco
        // 5 - Eletron Branco
        // 6 - Proton Branco
        public int[] TabuleiroVetor;
        private enum token {neutronPreto, eletronPreto, protonPreto,
                            neutronBranco, eletronBranco, protonBranco};

		protected int ObjectsSize = 15;
		private GameObjectCollection Pecas;
        private player token_jogador;
        private int token_posicao;
        private List<player> jogadores;
        private bool JogoEmAndamento = false;
        private bool JogadaCompleta = true;

        private Nodo ArvoreDecisao;

        const int MAX_VETOR = 137;
		#endregion

#region "Initializers"
		public Form1()
		{
			InitializeComponent();
            initGame();
        }
#endregion

#region "Events"
        private void button1_Click(object sender, EventArgs e) {
            jogadores = new List<player>();
            TabuleiroVetor = new int[137]{
                0, 0, 0, 1, 0, 0, 0,
                  0, 0, 3, 3, 0, 0,
                0, 0, 2, 2, 2, 0, 0,
                  0, 0, 3, 3, 0, 0,
                0, 0, 0, 2, 0, 0, 0,
                  0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0,
                  0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0,
                  0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0,
                  0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0,
                  0, 0, 0, 0, 0, 0,
                0, 0, 0, 0, 0, 0, 0,
                  0, 0, 0, 0, 0, 0,
                0, 0, 0, 5, 0, 0, 0,
                  0, 0, 6, 6, 0, 0,
                0, 0, 5, 5, 5, 0, 0,
                  0, 0, 6, 6, 0, 0,
                0, 0, 0, 4, 0, 0, 0 
            };

            token_posicao = 0;
            JogoEmAndamento = true;

            //Draw();
            criaPecas();

            player p1 = new player();
            p1.Jogador = Enums.pType.PC;
            p1.name = "PC";

            player p2 = new player();
            p2.Jogador = Enums.pType.humano;
            p2.name = "Humano";

            jogadores.Add(p1);
            jogadores.Add(p2);

            if(radioButton1.Checked){
                p1.Cor = Enums.pColor.branca;
                p2.Cor = Enums.pColor.preta;
                token_jogador = p1;
            }
            else{
                p2.Cor = Enums.pColor.branca;
                p1.Cor = Enums.pColor.preta;
                token_jogador = p2;
            }
            
            lblJogada.Text = token_jogador.name + " - peças " + token_jogador.Cor.ToString() + "s";

            listBoxUltimasJogadas.Items.Clear();

            ArvoreDecisao = new Nodo(TabuleiroVetor);
            ArvoreDecisao.filhos = Core.Core.addNodosArvore(ArvoreDecisao, token_jogador.Cor);

            if (token_jogador.Jogador == Enums.pType.PC)
                JogadaPC();
        }

        private void PecaClick(object sender, EventArgs e) {
            PictureBox peca = (PictureBox)sender;
            int position = getIndex(peca);

            if (!JogoEmAndamento) {
                MessageBox.Show("Clique em Iniciar jogo.");
                return;
            }
            if (Core.Core.excludeCasas.Contains(position)){
                MessageBox.Show("Clique em lugar válido no tabuleiro.");
                return;
            }
            if (TabuleiroVetor[position] == 0 && JogadaCompleta) {
                MessageBox.Show("Jogada proibida.");
                return;
            }
            if (TabuleiroVetor[position] > 0 && token_jogador.Cor != getPecaCor(position)) {
                MessageBox.Show("Clique na sua peça.");
                return;
            }

            // Se clicou numa peça
            if (TabuleiroVetor[position] > 0){
                // primeira desmarca tudo
                redesenhaTabuleiro();

                JogadaCompleta = false;

                // e então marca a peçca e aquelas no campo de visão
                peca.Image = GetBlinkedImage(position);
                foreach (int i in Core.Core.getCasasVisiveis(TabuleiroVetor, position))
                    getPeca(i).Image = GetBlinkedImage(i);
            }
            // Se clicou no vazio
            else {
                // exec jogada
                if (Core.Core.getCasasVisiveis(TabuleiroVetor, token_posicao).Contains(position)) {
                    TabuleiroVetor[position] = TabuleiroVetor[token_posicao];
                    TabuleiroVetor[token_posicao] = 0;
                    redesenhaTabuleiro();

                    if (PreparaProxJogada(position)) {
                        JogadaCompleta = true;
                        
                        listBoxUltimasJogadas.Items.Add("peca " + getPecaCor(position).ToString() + ": " + token_posicao.ToString() + " -> " + position.ToString());
                        ScrollHistoryToTheEnd();

                        RemovePecasCapturadas(getPecaCor(position));
                        CheckFinishGame();
                    }
                    else
                        MessageBox.Show("Sem jogadas disponíveis.");
                }
                else {
                    MessageBox.Show("Jogada proibida.");
                    return;
                }
            }
            //}

            // marca a peca clicada
            token_posicao = position;
            CheckCaptura();

            // verifica se eh a vez do PC e joga
            if (token_jogador.Jogador == Enums.pType.PC){
                JogadaPC();
                CheckCaptura();
            }
        }

        private void PecaHover(object sender, EventArgs e){
            PictureBox peca = (PictureBox)sender;
            peca.Cursor = Cursors.Hand;

            // e então marca aquelas no campo de visao
             //redesenhaTabuleiro();
            return;
            foreach (int i in Core.Core.getCasasVisiveis(TabuleiroVetor, getIndex(peca)))
                getPeca(i).Image = GetBlinkedImage(i);
        }

        private void PecaLeave(object sender, EventArgs e){
            PictureBox peca = (PictureBox)sender;
            peca.BackColor = TransparencyKey;
            peca.Cursor = Cursors.Default;
        }
#endregion

#region "Main Methods"
		protected bool initGame(){
			Pecas = new GameObjectCollection();
            JogoEmAndamento = false;
			return false;
		}

		protected bool criaPecas() {
			for (int i = 0; i < MAX_VETOR; i++) {
				PictureBox e = new PictureBox();
                e.Name = "peca" + i.ToString();
				e.Parent = panel1;
				e.BackColor = System.Drawing.Color.Transparent;
				e.Margin = new Padding(0);
				e.TabIndex = 0;
				e.Size = new Size(15, 15);
                e.Location = new Point(Positions.VetorPeca[i, 0] + panel1.Left, Positions.VetorPeca[i, 1] + panel1.Top);

                // Eventos
                e.Click += new System.EventHandler(this.PecaClick);
                e.MouseHover += new System.EventHandler(this.PecaHover);
                //e.MouseLeave += new System.EventHandler(this.PecaLeave);
                //e.Mouse += new System.EventHandler(this.PecaHover);

                // Imagem da Peca
                e.Image = GetImage(i);

                this.panel1.Controls.Add(e);
            }
			return false;
		}

        private Bitmap GetBlinkedImage(int i)
        {
            switch (TabuleiroVetor[i])
            {
                case 0: return Resources.casaVisivel;
                case 1: return Resources.tokenBlackNeutron1;
                case 2: return Resources.tokenBlackEletron1;
                case 3: return Resources.tokenBlackPositron1;
                case 4: return Resources.tokenWhiteNeutron1;
                case 5: return Resources.tokenWhiteEletron1;
                case 6: return Resources.tokenWhitePositron1;
            }

            return null;
        }

        private Bitmap GetImage(int i)
        {
            switch (TabuleiroVetor[i])
            {
                case 1: return Resources.tokenBlackNeutron;
                case 2: return Resources.tokenBlackEletron;
                case 3: return Resources.tokenBlackPositron;
                case 4: return Resources.tokenWhiteNeutron;
                case 5: return Resources.tokenWhiteEletron;
                case 6: return Resources.tokenWhitePositron;
            }
            return null;
        }

        private bool PreparaProxJogada(int position) {
            // Prepara p/ a próx jogada: (troca de jogador)
            // proximo jogador tem jogadas possiveis?

            // O codigo esta duplicado, mas assim eh mais rapido
            if (token_jogador.Cor == Enums.pColor.branca) {
                for (int j = 0; j < MAX_VETOR; j++) {
                    if (getPecaCor(j) == Enums.pColor.preta) {
                        if (Core.Core.getCasasVisiveis(TabuleiroVetor, j).Length > 0) {
                            if (token_jogador == jogadores.ElementAt(0))
                                token_jogador = jogadores.ElementAt(1);
                            else if (token_jogador == jogadores.ElementAt(1))
                                token_jogador = jogadores.ElementAt(0);
                            lblJogada.Text = token_jogador.name + " - peças " + token_jogador.Cor.ToString() + "s";

                            return true;
                        }
                    }
                }
            }
            else if (token_jogador.Cor == Enums.pColor.preta) {
                for (int j = 0; j < MAX_VETOR; j++) {
                    if (getPecaCor(j) == Enums.pColor.branca) {
                        if (Core.Core.getCasasVisiveis(TabuleiroVetor, j).Length > 0){
                            if (token_jogador == jogadores.ElementAt(0))
                                token_jogador = jogadores.ElementAt(1);
                            else if (token_jogador == jogadores.ElementAt(1))
                                token_jogador = jogadores.ElementAt(0);
                            lblJogada.Text = token_jogador.name + " - peças " + token_jogador.Cor.ToString() + "s";

                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool CheckFinishGame() {
            int[] pretas = new int[3] { 0, 0, 0 };
            int[] brancas = new int[3] { 0, 0, 0 };

            for (int j = 0; j < MAX_VETOR; j++) {
                switch (TabuleiroVetor[j]) {
                    case 1: pretas[0]++; break;
                    case 2: pretas[1]++; break;
                    case 3: pretas[2]++; break;
                    case 4: brancas[0]++; break;
                    case 5: brancas[1]++; break;
                    case 6: brancas[2]++; break;
                    }
                }

            player jogadorVencedor = new player();

            if (pretas[0] == 0 || pretas[1] == 0 || pretas[2] == 0) {
                foreach (player jogador in jogadores) {
                    if (jogador.Cor == Enums.pColor.branca) {
                        jogadorVencedor = jogador;
                        break;
                        }
                    }
                MessageBox.Show(jogadorVencedor.name + " ganhou! (peças bancas)");
                JogoEmAndamento = false;
                return true;
                }
            else if (brancas[0] == 0 || brancas[1] == 0 || brancas[2] == 0) {
                foreach (player jogador in jogadores) {
                    if (jogador.Cor == Enums.pColor.preta) {
                        jogadorVencedor = jogador;
                        break;
                        }
                    }
                MessageBox.Show(jogadorVencedor.name + " ganhou! (peças pretas)");
                JogoEmAndamento = false;
                return true;
                }

            return false;
            }

    #region "AI"
        private int EscolhePecaPc(Enums.pColor cor)
        {
            int[] posicaoPecas = getPecasPosicao(cor);
            int pecaEscolhida = 0;
            Random random = new Random();
            do {
                pecaEscolhida = posicaoPecas[random.Next(0, posicaoPecas.Length - 1)];
            } while (Core.Core.getCasasVisiveis(TabuleiroVetor, pecaEscolhida).Length == 0);

            return pecaEscolhida;
        }

        private void JogadaPC()
        {
            if (JogoEmAndamento)
            {
                int position = 0;
                bool jogadaOk = false;
                int qtdTentativasJogadas = 0;
                int indicePositionCapturaObrigatoria = 0;
                
                if (CapturaObrigatoria(token_jogador.Cor))
                    indicePositionCapturaObrigatoria = RealizaCapturaObrigatoria(token_jogador.Cor);

                //Verifica se é obrigado a capturar alguma peça e tem como realizar a captura
                if (indicePositionCapturaObrigatoria != 0)                    
                    position = indicePositionCapturaObrigatoria;
                else
                {
                    //do {
                    //    qtdTentativasJogadas++;

                    //    // qualquer jogada...
                    //    if (!jogadaOk && qtdTentativasJogadas > 100) {
                    //        token_posicao = EscolhePecaPc(token_jogador.Cor);
                    //        foreach (int peca in Core.Core.getCasasVisiveis(TabuleiroVetor, token_posicao)) {
                    //            //SELECIONA QUALQUER CASA PARA SE MOVER E VAI CAVALO
                    //            position = peca; //no momento pega a ultima casa avaliada
                    //        }
                    //        jogadaOk = true;
                    //    }

                    //} while (!jogadaOk);


                    Enums.pColor rivalCor = (token_jogador.Cor == Enums.pColor.branca) ? Enums.pColor.preta : Enums.pColor.branca;

                    ArvoreDecisao = new Nodo(TabuleiroVetor);
                    ArvoreDecisao.filhos = Core.Core.addNodosArvore(ArvoreDecisao, token_jogador.Cor);
                    for (int i = 0; i < ArvoreDecisao.filhos.Length; i++) {
                        ArvoreDecisao.filhos[i].filhos = Core.Core.addNodosArvore(ArvoreDecisao.filhos[i], rivalCor);
                    }
                    
                    int iBest = position;
                    int iBestFrom = token_posicao;
                    int bestCount = 0;
                    foreach (Nodo filho in ArvoreDecisao.filhos) {
                        if (filho.MovePara > 0) {
                            int minmaxNodo = Core.Core.Negamax(filho, (int)numericUpDown1.Value, 10, 15, -10);
                            if (minmaxNodo > bestCount) {
                                bestCount = minmaxNodo;
                                iBest = filho.MovePara;
                                iBestFrom = filho.MoveDe;
                            }
                        }
                    }
                    token_posicao = iBestFrom;
                    position = iBest;

                    //move a peca
                    TabuleiroVetor[position] = TabuleiroVetor[iBestFrom];
                    TabuleiroVetor[iBestFrom] = 0;
                }
                redesenhaTabuleiro();

                listBoxUltimasJogadas.Items.Add(String.Format("peca {0}: {1} -> {2}",
                                                getPecaCor(position).ToString(), token_posicao.ToString(), position.ToString()));
                ScrollHistoryToTheEnd();

                RemovePecasCapturadas(getPecaCor(position));
                CheckFinishGame();


                if (PreparaProxJogada(position) && token_jogador.Jogador == Enums.pType.PC)
                    JogadaPC();

                JogadaCompleta = true;
            }
        }

        private Boolean CapturaObrigatoria(Enums.pColor Cor)
        {
            Enums.pColor CorAdversario = Enums.pColor.undefined;

            if (Cor == Enums.pColor.branca)
                CorAdversario = Enums.pColor.preta;
            else
                CorAdversario = Enums.pColor.branca;

            foreach (int indicePeca in getPecasPosicao(CorAdversario))
            {
                if (PecaCapturavel(indicePeca, CorAdversario))
                    return true;
            }

            return false;
        }

        private int RealizaCapturaObrigatoria(Enums.pColor Cor)
        {
            Enums.pColor CorAdversario = Enums.pColor.undefined;

            if (Cor == Enums.pColor.branca)
                CorAdversario = Enums.pColor.preta;
            else
                CorAdversario = Enums.pColor.branca;

            foreach (int indicePeca in getPecasPosicao(CorAdversario))
            {
                if (PecaCapturavel(indicePeca, CorAdversario))
                {
                    foreach (int indicePecaCaptura in getPecasPosicao(Cor))
                    {
                        if (ExisteCasaComum(indicePeca, indicePecaCaptura))
                        {
                            foreach (int casaComum in CasasComuns(indicePeca, indicePecaCaptura))
                            {
                                int indicePecaCapturaAntiga = indicePecaCaptura;

                                TabuleiroVetor[casaComum] = TabuleiroVetor[indicePecaCapturaAntiga];
                                TabuleiroVetor[indicePecaCapturaAntiga] = 0;

                                if (PecaCapturavel(indicePeca, CorAdversario))
                                {
                                    token_posicao = indicePecaCapturaAntiga;
                                    return casaComum;
                                }
                                else
                                {
                                    TabuleiroVetor[indicePecaCapturaAntiga] = TabuleiroVetor[casaComum];
                                    TabuleiroVetor[casaComum] = 0;
                                }
                            }
                        }
                    }
                }
            }

            return 0;
        }

        private void RemovePecasCapturadas(Enums.pColor Cor)
        {
            List<int> IndicesPecasRemovidas = new List<int>();
            Enums.pColor CorAdversario = Enums.pColor.undefined;
            
            if (Cor == Enums.pColor.branca)
                CorAdversario = Enums.pColor.preta;
            else
                CorAdversario = Enums.pColor.branca;

            foreach (int indicePeca in getPecasPosicao(CorAdversario))
            {
                if (PecaCapturavel(indicePeca, CorAdversario))
                    IndicesPecasRemovidas.Add(indicePeca);
            }            
            if (IndicesPecasRemovidas.Count > 0)
            {
                foreach (int IndicePeca in IndicesPecasRemovidas)
                {
                    listBoxUltimasJogadas.Items.Add("peca " + CorAdversario.ToString() + " capturada: " + IndicePeca.ToString());
                    ScrollHistoryToTheEnd();
                    TabuleiroVetor[IndicePeca] = 0;
                }
                redesenhaTabuleiro();
            }
        }
        
        private int PecaCercavel(Enums.pColor cor)
        {
            Enums.pColor corAdversario;
            if (cor == Enums.pColor.branca)
                corAdversario = Enums.pColor.preta;
            else
                corAdversario = Enums.pColor.branca;

            foreach (int pecaAdversario in getPecasPosicao(corAdversario))
                if (PecaCapturavelComMaisUmMovimento(pecaAdversario))
                    return IndicePecaCapturavelComMaisUmMovimento(pecaAdversario);

            return 0;
        }

        private bool PecaAdversarioCercado(Enums.pColor cor)
        {
            bool algumaPecaCercadaAdversario = false;
            Enums.pColor corAdversario;
            if (cor == Enums.pColor.branca)
                corAdversario = Enums.pColor.preta;
            else
                corAdversario = Enums.pColor.branca;

            foreach (int pecaAdversario in getPecasPosicao(corAdversario))
            {
                if (PecaCapturavelComMaisUmMovimento(pecaAdversario))
                {
                    algumaPecaCercadaAdversario = true;
                    break;
                }
            }

            return algumaPecaCercadaAdversario;
        }

        private int IndicePecaCapturavelComMaisUmMovimento(int peca)
        {
            int qtd = 0;
            int cargasSoma = 0;
            Enums.pColor cor;
            List<int> pecasQueCercam = new List<int>();

            if (getPecaCor(peca) == Enums.pColor.branca)
                cor = Enums.pColor.preta;
            else
                cor = Enums.pColor.branca;

            foreach (int i in Core.Core.getPecasVisiveis(TabuleiroVetor, peca))
            {
                cargasSoma += getParticulaCarga(i);
                if (getPecaCor(i) == cor)
                {
                    pecasQueCercam.Add(i);
                    qtd++;
                }
            }

            if ((qtd > 1) && ((cargasSoma == -1) || (cargasSoma == 1)))
            {
                foreach (int casa in Core.Core.getCasasVisiveis(TabuleiroVetor, peca))
                {
                    foreach (int pecaQuePodeCercar in getPecasPosicao(cor))
                    {
                        if (!pecasQueCercam.Contains(pecaQuePodeCercar) && ExisteCasaComum(peca, pecaQuePodeCercar))
                        {
                            token_posicao = pecaQuePodeCercar;
                            return CasaComum(peca, pecaQuePodeCercar);
                        }
                    }
                }
            }

            return 0;
        }

        private int CasaComum(int peca, int pecaQuePodeCercar)
        {
            foreach (int casasVisiveisPeca in Core.Core.getCasasVisiveis(TabuleiroVetor, peca))
                foreach (int casasVisiveisPecaQuePodeCercar in Core.Core.getCasasVisiveis(TabuleiroVetor, pecaQuePodeCercar))
                    if (casasVisiveisPeca == casasVisiveisPecaQuePodeCercar)
                        return casasVisiveisPeca;
            return 0;
        }

        private List<int> CasasComuns(int peca, int pecaQuePodeCercar)
        {
            List<int> casasEmComum = new List<int>();
            foreach (int casasVisiveisPeca in Core.Core.getCasasVisiveis(TabuleiroVetor, peca))
                foreach (int casasVisiveisPecaQuePodeCercar in Core.Core.getCasasVisiveis(TabuleiroVetor, pecaQuePodeCercar))
                    if (casasVisiveisPeca == casasVisiveisPecaQuePodeCercar)
                        casasEmComum.Add(casasVisiveisPeca);
            return casasEmComum;
        }

        private bool PecaCapturavelComMaisUmMovimento(int peca)
        {
            int qtd = 0;
            int cargasSoma = 0;
            Enums.pColor cor;
            bool result = false;
            List<int> pecasQueCercam = new List<int>();

            if (getPecaCor(peca) == Enums.pColor.branca)
                cor = Enums.pColor.preta;
            else
                cor = Enums.pColor.branca;

            foreach (int i in Core.Core.getPecasVisiveis(TabuleiroVetor, peca))
            {
                cargasSoma += getParticulaCarga(i);
                if (getPecaCor(i) == cor)
                {
                    pecasQueCercam.Add(i);
                    qtd++;
                }
            }

            if ((qtd > 1) && ((cargasSoma == -1) || (cargasSoma == 1)))
            {
                foreach (int casa in Core.Core.getCasasVisiveis(TabuleiroVetor, peca))
                {
                    foreach (int pecaQuePodeCercar in getPecasPosicao(cor))
                    {
                        if (!pecasQueCercam.Contains(pecaQuePodeCercar) && ExisteCasaComum(peca, pecaQuePodeCercar))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }

            return result;
        }

        private bool ExisteCasaComum(int peca, int pecaQuePodeCercar)
        {
            foreach (int casasVisiveisPeca in Core.Core.getCasasVisiveis(TabuleiroVetor, peca))
                foreach (int casasVisiveisPecaQuePodeCercar in Core.Core.getCasasVisiveis(TabuleiroVetor, pecaQuePodeCercar))
                    if (casasVisiveisPeca == casasVisiveisPecaQuePodeCercar)
                        return true;
            return false;
        }

        private int PosicaoSegura(int posPeca)
        {
            bool posSegura = false;
            int[] TabuleiroAux = new int[MAX_VETOR];
            int posicaoSegura = 0;
            do
            {
                foreach (int novaPos in Core.Core.getCasasVisiveis(TabuleiroVetor, posPeca))
                {
                    TabuleiroVetor.CopyTo(TabuleiroAux, 0);
                    TabuleiroAux[novaPos] = TabuleiroAux[posPeca];
                    TabuleiroAux[posPeca] = 0;

                    if (!PecaCapturavel(novaPos, getPecaCor(posPeca), TabuleiroAux))
                    {
                        posSegura = true;
                        posicaoSegura = novaPos;
                    }
                }
            } while (!posSegura);

            return posicaoSegura;
        }

        private int PecaEmRisco(Enums.pColor cor)
        {
            foreach (int peca in getPecasPosicao(cor))
            {
                if (PecaCapturavel(peca, cor))
                    return peca;
            }
            return 0;
        }

        private bool PecasEmRisco(Enums.pColor cor)
        { 
            foreach (int peca in getPecasPosicao(cor))
            {
                if (PecaCapturavel(peca, cor))
                    return true;
            }
            return false;
        }
    #endregion
        
    #region "Catpura"
        /// Se a peça for capturável, executa captura.
        private bool CheckCaptura() {
        if (JogadaCompleta) {
            foreach (int i in getPecasPosicao(token_jogador.Cor)) {
                if (PecaCapturavel(i, token_jogador.Cor)) {
                    TabuleiroVetor[i] = 0;
                    CheckFinishGame();
                    return true;
                    }
                }
            }
            return false;
        }

        private bool PecaCapturavel(int pos, Enums.pColor cor) {
            int qtd = 0;
            int cargasSoma = 0;

            foreach (int i in Core.Core.getPecasVisiveis(TabuleiroVetor, pos)) {
                cargasSoma += getParticulaCarga(i);
                if (getPecaCor(i) == Enums.pColor.preta && cor == Enums.pColor.branca)
                    qtd++;
                else if (getPecaCor(i) == Enums.pColor.branca && cor == Enums.pColor.preta)
                    qtd++;
            }

            return (qtd > 1) && (cargasSoma == 0);
        }

        private bool PecaCapturavel(int pos, Enums.pColor cor, int[] tabuleiro){
            int qtd = 0;
            int cargasSoma = 0;

            foreach (int i in Core.Core.getPecasVisiveis(tabuleiro, pos))
            {
                cargasSoma += getParticulaCarga(i);
                if (getPecaCor(i) == Enums.pColor.preta && cor == Enums.pColor.branca)
                    qtd++;
                else if (getPecaCor(i) == Enums.pColor.branca && cor == Enums.pColor.preta)
                    qtd++;
            }

            return (qtd > 1) && (cargasSoma == 0);
        }
    #endregion
#endregion

    #region "Auxiliar Methods"
        /// <summary>
        /// Retorna a Peça que está no índice i.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        private PictureBox getPeca(int i)
        {
            return (PictureBox)this.Controls.Find("peca" + i.ToString(), true).First();
        }

        /// <summary>
        /// Retorna o índice no tabuleiro da peça indicada.
        /// </summary>
        /// <param name="peca"></param>
        /// <returns></returns>
        private int getIndex(PictureBox peca) {
            return Convert.ToInt32(peca.Name.Replace("peca", ""));
        }

        private Core.Enums.pColor getPecaCor(int i) {
            if (TabuleiroVetor[i] > 0 && TabuleiroVetor[i] < 4)
                return Enums.pColor.preta;
            else if(TabuleiroVetor[i] > 3)
                return Enums.pColor.branca;
            return Enums.pColor.undefined;
        }

        /// <summary>
        /// Retorna a carga de uma particula indicada pelo tipo de peça no tabuleiro.
        /// </summary>
        /// <returns></returns>
        private int getParticulaCarga(int i) {
            switch (TabuleiroVetor[i]) {
                case 2: case 5: return -1;
                case 3: case 6: return 1;
            }
            return 0;
        }

        private bool redesenhaTabuleiro()
        {
            for (int i = 0; i < MAX_VETOR; i++)
            {
                PictureBox ipeca = (PictureBox)this.Controls.Find("peca" + i.ToString(), true).First();
                if (ipeca != null)
                {
                    ipeca.Image = GetImage(i);
                }

            }
            return false;
        }

        private String ListaItens(int[] Tabuleiro)
        {
            String lista = "";

            foreach (int num in Tabuleiro)
            {
                lista += num.ToString() + " ";
            }

            return lista;
        }

        /// <summary>
        /// Retorna a posição das peças de uma determinada cor existentes no tabuleiro.
        /// </summary>
        /// <param name="tabuleiroVetor">Tabuleiro.</param>
        /// <param name="cor">Cor das peças que se deseja.</param>
        /// <returns></returns>
        private int[] getPecasPosicao(Enums.pColor cor)
        {
            List<int> result = new List<int>();

            for (int i = 0; i < MAX_VETOR; i++){
                if (getPecaCor(i) == cor)
                    result.Add(i);
            }

            return result.ToArray();
        }
    #endregion

#region "UI Methods"
        private void ScrollHistoryToTheEnd() {
            int visibleItems = listBoxUltimasJogadas.ClientSize.Height / listBoxUltimasJogadas.ItemHeight;
            listBoxUltimasJogadas.TopIndex = Math.Max(listBoxUltimasJogadas.Items.Count - visibleItems + 1, 0);
        }
#endregion

    }
}
