using Cadastre.Data.Enumerations;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cadastre.Data.Models
{
    public class District
    {
        public District()
        {
            this.Properties = new HashSet<Property>();
        }

        [PrimaryKey]
        public int Id { get; set; }

        [MinLength(ValidationConstraints.DistrictNameMinLength)]
        [MaxLength(ValidationConstraints.DistrictNameMaxLength)]
        public string Name { get; set; } = null!;


        [MaxLength(ValidationConstraints.DistrictPostalCodeLength)]
        [MinLength(ValidationConstraints.DistrictPostalCodeLength)]
        [RegularExpression(ValidationConstraints.DistrictPostalCodeFormat)]
        public string PostalCode { get; set; } = null!;

        public Region Region { get; set; }

        public virtual ICollection<Property> Properties { get; set; }


    }
}
