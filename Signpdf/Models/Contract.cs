using System;
using System.Collections.Generic;

namespace Signpdf.Models
{
    public partial class Contract
    {
        public Contract()
        {
            Signatures = new HashSet<Signature>();
        }

        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public bool? IsSigned { get; set; }
        public string Title { get; set; } = null!;
        public string Path { get; set; } = null!;

        public virtual ICollection<Signature> Signatures { get; set; }
    }
}
