using Signpdf.Controllers;

namespace Signpdf.Models
{
    public interface ISignService
    {
        void SignMany(string destPath, List<Signature> signatures, Contract contract);
    }
}
