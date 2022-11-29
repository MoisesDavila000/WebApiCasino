using Microsoft.EntityFrameworkCore;
using System;
using WebApiCasino.Entidades;

namespace WebApiCasino.Servicios
{
    public interface IService
    {
        List<CartasLoteMex> EjecutarRandom(List<CartasLoteMex> cartasUsadas, int numganadores);
    }

    public class ServicioRandom : IService
    {
        public List<CartasLoteMex> EjecutarRandom(List<CartasLoteMex> cartasUsadas, int numganadores)
        {
            Random random = new Random();
            List<CartasLoteMex> lista = new List<CartasLoteMex>();
            List<int> repetir = new List<int>();
            int index = 0;
            int contador = 0;

            for (int i = 0; i < numganadores; i++)
            {
                do
                {
                    contador = 0;
                    index = random.Next(0, cartasUsadas.Count());
                    foreach (var j in repetir)
                    {
                        if (index == j)
                        {
                            contador++;
                            break;
                        }
                    }


                } while (contador != 0);

                lista.Add(cartasUsadas[index]);
                repetir.Add(index);
            }

            return (lista);
        }
    }
}
