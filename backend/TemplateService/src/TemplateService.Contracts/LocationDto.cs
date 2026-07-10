using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TemplateService.Contracts
{
        public record CreateLocationDto(Guid Id, string Name, string Address);
}