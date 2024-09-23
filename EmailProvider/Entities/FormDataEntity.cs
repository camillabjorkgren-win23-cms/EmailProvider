using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailProvider.Entities;
public class FormDataEntity
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string? Name { get; set; }
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Message { get; set; }
    public string? Subject { get; set; }
    public string PartitionKey { get; set; } = null!;

}
