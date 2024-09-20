using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailProvider.Models;
public class FormData
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Message { get; set; }
    public string? Subject { get; set; }
    public string? Phone{ get; set; }
}
