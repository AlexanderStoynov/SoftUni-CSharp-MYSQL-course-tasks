using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cadastre.Data
{
    public static class ValidationConstraints
    {
        public const int DistrictNameMinLength = 2;
        public const int DistrictNameMaxLength = 80;
        public const int DistrictPostalCodeLength = 8;
        public const string DistrictPostalCodeFormat = "[A-Z]{2}-[0-9]{5}";

        public const int PropertyIdentifierMinLength = 16;
        public const int PropertyIdentifierMaxLength = 20;

        public const int PropertyAreaMinRange = 1;
        public const int PropertyAreaMaxRange = int.MaxValue;
        public const int PropertyDetailsMinLength = 5;
        public const int PropertyDetailsMaxLength = 500;
        public const int PropertyAddressMinLength = 5;
        public const int PropertyAddressMaxLength = 500;

        public const int CitizenFirstNameMinLength = 2;
        public const int CitizenFirstNameMaxLength = 30;
        public const int CitizenLastNameMinLength = 2;
        public const int CitizenLastNameMaxLength = 30;



    }
}
