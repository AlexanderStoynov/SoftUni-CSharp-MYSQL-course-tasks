using Cadastre.Data.Enumerations;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForeignKeyAttribute = System.ComponentModel.DataAnnotations.Schema.ForeignKeyAttribute;

namespace Cadastre.Data.Models
{
    public class Citizen
    {
        public Citizen() 
        {
            this.PropertiesCitizens = new HashSet<PropertyCitizen>();
        }

        [PrimaryKey]
        public int Id { get; set; }

        [StringLength(ValidationConstraints.CitizenFirstNameMinLength, ValidationConstraints.CitizenFirstNameMaxLength)]
        public string FirstName { get; set; } = null!;

        [StringLength(ValidationConstraints.CitizenLastNameMinLength, ValidationConstraints.CitizenLastNameMaxLength)]
        public string LastName { get; set; } = null!;

        public DateTime BirthDate { get; set; }

        [ForeignKey(nameof(District))]
        public int DistrictId { get; set; }

        public virtual District District { get; set; }

        public MaritalStatus MaritalStatus { get; set; }

        public virtual ICollection<PropertyCitizen> PropertiesCitizens { get; set; }
    }
}
