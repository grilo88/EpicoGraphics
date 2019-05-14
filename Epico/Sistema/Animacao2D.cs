using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epico
{
    public class Quadro
    {
        public float x;
        public float y;
    }

    public class Animacao2D
    {
        public string Nome { get; set; }
        public int QuadrosPorSegundo { get; private set; }
        public int DuracaoMilisegundos { get; private set; } // Total do tempo da animação em milisegundos
        public int MilisegundoQuadro { get; private set; }  // Milisegundos de cada quadro

        public List<Quadro> percurso = new List<Quadro>();

        public void AddQuadro(Quadro quadro)
        {
            percurso.Add(quadro);
        }

        public void GerarAnimacao(
            int quadrosPorSegundo, int duracaoMilisegundos, IEfeito efeito, IInteracao interacao)
        {
            this.QuadrosPorSegundo = quadrosPorSegundo;
            this.DuracaoMilisegundos = duracaoMilisegundos;
        }
    }

    public interface IInteracao
    {

    }

    public interface IEfeito
    {

    }
}
