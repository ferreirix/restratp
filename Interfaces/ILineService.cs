using System.Threading.Tasks;
using restratp.Models;

namespace restratp.Interfaces
{
    public interface ILineService
    {
         Task<LineModel[]> GetNetworkLines(string networkId);
         Task<string> GetLineColor(string networkId, string lineId);
    }
}