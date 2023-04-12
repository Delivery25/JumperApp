using System;
using System.Linq;
using System.Windows.Media;

namespace JumperApp.Model
{
    public partial class Agent
    {
        public int CountSales
        {
            get
            {
                try
                {
                    using (var context = new JumperEntities())
                    {
                        var year = DateTime.Now.AddYears(-1).Date;
                        return context.ProductSale.Count(x => x.AgentID == ID && x.SaleDate > year);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public Color ColorAgent => Discount > 25 ? Colors.LightGreen : Colors.White;

        public int Discount
        {
            get
            {
              if(CountSales < 10000)
                  return 0;
              if(CountSales < 50000)
                  return 5;
              if(CountSales < 150000)
                  return 10;
              if (CountSales < 500000)
                  return 20;
              if (CountSales > 500000)
                  return 25;
              return 0;
            }
        }

    }
}
