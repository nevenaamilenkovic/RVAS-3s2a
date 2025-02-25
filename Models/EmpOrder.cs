using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Tsql3s2a.Models;

[Keyless]
public partial class EmpOrder
{
    [Column("empid")]
    public int Empid { get; set; }

    [Column("ordermonth", TypeName = "datetime")]
    public DateTime? Ordermonth { get; set; }

    [Column("qty")]
    public int? Qty { get; set; }

    [Column("val", TypeName = "numeric(12, 2)")]
    public decimal? Val { get; set; }

    [Column("numorders")]
    public int? Numorders { get; set; }
}
