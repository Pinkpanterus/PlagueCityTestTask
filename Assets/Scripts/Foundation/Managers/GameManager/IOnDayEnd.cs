using System.Threading.Tasks;
using Foundation;

namespace Game
{
    public interface IOnDayEnd
    {
        Task Do();
    }
}