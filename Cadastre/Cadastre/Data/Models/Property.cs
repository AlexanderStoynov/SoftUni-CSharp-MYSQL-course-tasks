using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForeignKeyAttribute = System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute;
using RangeAttribute = System.ComponentModel.DataAnnotations.RangeAttribute;
using StringLengthAttribute = ServiceStack.DataAnnotations.StringLengthAttribute;

namespace Cadastre.Data.Models
{
    public class Property
    {
        public Property () 
        {
            this.PropertiesCitizens = new HashSet<PropertyCitizen>();
        }

        [PrimaryKey]
        public int Id { get; set; }

        [MinLength(ValidationConstraints.PropertyIdentifierMinLength)]
        [MaxLength(ValidationConstraints.PropertyIdentifierMaxLength)]
        public string PropertyIdentifier { get; set; } = null!;

        [Range(ValidationConstraints.PropertyAreaMinRange, ValidationConstraints.PropertyAreaMaxRange)]
        public int Area { get; set; }

        [StringLength(ValidationConstraints.PropertyDetailsMinLength, ValidationConstraints.PropertyDetailsMaxLength)]
        public string Details { get; set; } = null!;

        [StringLength(ValidationConstraints.PropertyAddressMinLength, ValidationConstraints.PropertyAddressMaxLength)]
        public string Address { get; set; } = null!;

        public DateTime DateOfAcquisition { get; set; }

        [ForeignKey(nameof(District))]
        public int DistrictId { get; set; }

        public virtual District District { get; set; } = null!;

        public virtual ICollection<PropertyCitizen> PropertiesCitizens { get; set; }
    }
}
