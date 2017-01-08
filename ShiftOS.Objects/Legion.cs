using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum LegionRole
  {
      Admin,
      Manager,
      Committed,
      Trainee,
      AwaitingInvite
  }

  public enum LegionPublicity
  {
      Public, //Will display on the 'Join Legion' page, anyone can join
      PublicInviteOnly, //Will display on the 'Join Legion' page but you must be invited
      Unlisted, //Won't display on 'Join Legion', but anyone can join
      UnlistedInviteOnly //Won't display in 'Join Legion', and admin/manager invitation is required.
  }

  public class Legion
  {
      public string Name { get; set; }
      public LegionPublicity Publicity { get; set; }
      public ConsoleColor BannerColor { get; set; }
      public string Description { get; set; }
      public string ShortName { get; set; }

      public Dictionary<string, LegionRole> Roles { get; set; }
      public Dictionary<LegionRole, string> RoleNames { get; set; }


  }
