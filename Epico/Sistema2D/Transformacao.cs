using EpicoGraphics.Sistema2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EpicoGraphics
{
    public class Transformacao
    {
        readonly Objeto2D obj;

        public Transformacao(Objeto2D obj)
        {
            this.obj = obj;
        }

        /// <summary>
        /// Deslocamento do objeto no espaço
        /// </summary>
        public void Translacao()
        {
            // TODO: Translação
        }

        /// <summary>
        /// Redimensionamento escalar do objeto
        /// </summary>
        public void Escala()
        {
            // TODO: Escala
        }

        /// <summary>
        /// Rotação em torno do ponto central do objeto
        /// </summary>
        public void Rotacao()
        {
            // TODO: Rotação
        }

        /// <summary>
        /// Espelhamento do objeto
        /// </summary>
        public void Reflexao()
        {
            // TODO: Reflexão
        }

        /// <summary>
        /// Deformação do objeto
        /// </summary>
        public void Cizalhamento()
        {
            // TODO: Cizalhamento
        }
    }
}
