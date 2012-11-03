using System;
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
	public partial class Form1 : Form
	{
		#region "Properties, Globals"
        // 1 - Neutron Preto
        // 2 - Eletron Preto
        // 3 - Proton Preto
        // 4 - Neutron Branco
        // 5 - Eletron Branco
        // 6 - Proton Branco
		public int[] TabuleiroVetor = new int[115] {
			  0, 0, 1, 0, 0, 
			0, 0, 3, 3, 0, 0,
			  0, 2, 2, 2, 0, 
			0, 0, 3, 3, 0, 0,
			  0, 0, 2, 0, 0, 
			0, 0, 0, 0, 0, 0,
			  0, 0, 0, 0, 0, 
			0, 0, 0, 0, 0, 0,
			  0, 0, 0, 0, 0, 
			0, 0, 0, 0, 0, 0,
			  0, 0, 0, 0, 0, 
			0, 0, 0, 0, 0, 0,
			  0, 0, 0, 0, 0, 
			0, 0, 0, 0, 0, 0,
			  0, 0, 0, 0, 0, 
			0, 0, 0, 0, 0, 0,
			  0, 0, 5, 0, 0, 
			0, 0, 6, 6, 0, 0,
			  0, 5, 5, 5, 0, 
			0, 0, 6, 6, 0, 0,
			  0, 0, 4, 0, 0
		};
        private enum player { PC, humano };
        private enum token {neutronPreto, eletronPreto, protonPreto,
                            neutronBranco, eletronBranco, protonBranco};

		protected int ObjectsSize = 15;
		private GameObjectCollection Pecas;
        private player token_jogador = player.PC;
        private int token_posicao = 0;
		#endregion

		#region "Initializers"
		public Form1()
		{
			InitializeComponent();
			initGame();
		}
		#endregion

		#region "Events"
        private void PecaClick(object sender, EventArgs e){
            PictureBox peca = (PictureBox)sender;
            int position = getIndex(peca);

            if (Core.Core.excludeCasas.Contains(position)){
                MessageBox.Show("Clique em lugar válido no tabuleiro.");
                return;
            }

            // Se tem alguma peca...
            if (TabuleiroVetor[position] > 0) {
                // primeira desmarca tudo
                redesenhaTabuleiro();

                peca.Image = GetBlinkedImage(position);

                // e então marca aquelas no campo de visao
                foreach (int i in Core.Core.getCasasVisiveis(TabuleiroVetor, position))
                    getPeca(i).Image = GetBlinkedImage(i);
            }
            // Se está vazio
            else {
                TabuleiroVetor[position] = TabuleiroVetor[token_posicao];
                TabuleiroVetor[token_posicao] = 0;
                redesenhaTabuleiro();
            }

            // marca a peca clicada
            token_posicao = position;
        }

        private void PecaHover(object sender, EventArgs e){
            PictureBox peca = (PictureBox)sender;
            peca.Cursor = Cursors.Hand;

            // e então marca aquelas no campo de visao
            redesenhaTabuleiro();
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

            if (criaPecas())
				return true;
			return false;
		}

		protected bool criaPecas() {
			for (int i = 0; i < 115; i++) {
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

        /// <summary>
        /// Retorna a carga de uma particula indicada pelo tipo de peça no tabuleiro.
        /// </summary>
        /// <returns></returns>
        private int getParticulaCarga(int i) {
            switch (i) {
                case 1: case 4: return -1;
                case 2: case 5: return 1;
            }
            return 0;
        }

        private bool redesenhaTabuleiro()
        {
            for (int i = 0; i < 115; i++)
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
        #endregion

    }
}
