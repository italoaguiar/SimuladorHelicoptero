using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Linguagens
{
    public class Lista2
    {
        //1 - A)
        public uint soma(uint n)
        {
            uint k = 0;
            for (uint i = 0; i <= n; i++)
                k += i;
            return k;
        }
        //1 - B)
        public uint sub(uint x, uint y)
        {
            int k = (int)x;
            for(uint i = 0; i<y; i++)
            {
                if (k < 0) return i;
                k -= (int)i;
            }
            return 0;
        }
        //1 - C)
        public int mdc(int a, int b)
        {
            if (b == 0)
                return a;
            else
                return mdc(b, a % b);
        }
        //1 - D)
        public double cos(int x, int k)
        {
            double f = 0;
            for(int i = 0; i< k; i++)
            {
                f += (Math.Pow(-1, i) / fatorial(2 * i)) * Math.Pow(x, 2 * i);
            }
            return f;
        }
        public int fatorial(int i)
        {
            if (i <= 1)
                return 1;
            return i * fatorial(i - 1);
        }


        // 2 - 
        // s = p(a, f(a))
        // t = p(a, f(a))

        // 2- A) Sim
        // 2- B) {X/Y, Y/Z, Z/a}

        // 3 - É uma propriedade

        // 4 - É uma propriedade

        // 5 -
        // A) A = b
        // B) F = joao, G = maria
        // C) A = g(a), B = (g(g(a)), X = b
        // D) X = g(c), Y = c,  Z = g(c)
        // E) X = h(a), W = h(h(a)), Z = h(a)

        // 6 -
        // p(X) = p(p(X))
        // X = p(X)
        // p(p(x)) = p(p(p(X)
        // p(p(p(X) = p(p(p(p(x)
        // ...
        // O algoritmo executa uma série de substituições
        // recursivas que fazem com que o termo gerado tenha
        // tamanho infinito.

        // 7 -
        // A)
        // p -> q
        // !p
        // p
        //
        // B)
        // t ^ r -> s (1)
        // t    p ^ s -> r
        //      p   s (1)  

        // 8-
        // A)
        // c(a,e) <- r(a,Z) ^ r(Z,e)
        //           r(a,b) ^ r(b,e)
        //                      F
        // B)
        //                 c(a,e)
        //      ______________|_______________
        //      |                            |
        //    r(a,e)                  r(a,Z) ^ r(Z,e)
        //      F                  __________|__________
        //                         |                    |
        //                       r(a,b)              r(b,e)
        //                                              F
    }
}
