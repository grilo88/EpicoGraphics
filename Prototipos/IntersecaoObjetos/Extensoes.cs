using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntersecaoObjetos
{
    public static class Extensoes
    {
        private const double Epsilon = 1e-10;

        /// <summary>
        /// Ao trabalhar com números de ponto flutuante em computadores, é comum haver alguns erros de arredondamento, o que dificulta quando, por exemplo, você deseja testar se um número é igual a zero. Portanto, escolhemos uma constante muito pequena para testar e, se nosso número for menor que essa constante, consideramos zero. Aqui, eu fiz um pequeno método de extensão para nos ajudar.
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        public static bool IsZero(this double d)
        {
            return Math.Abs(d) < Epsilon;
        }
    }
}
