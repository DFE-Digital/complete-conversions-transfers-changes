using Dfe.Complete.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dfe.Complete.Application.Mappers
{
    public class ProjectTeamPresentationMapper
    {
        public static string? Map(ProjectTeam? projectTeam)
        {
            if(projectTeam == null)
                return null;

            switch (projectTeam)
            {
                case ProjectTeam.London:
                    return "London";
                case ProjectTeam.SouthEast:
                    return "South East";
                case ProjectTeam.YorkshireAndTheHumber:
                    return "Yorkshire and the Humber";
                case ProjectTeam.NorthWest:
                    return "North West";
                case ProjectTeam.EastOfEngland:
                    return "East of England";
                case ProjectTeam.WestMidlands:
                    return "West Midlands";
                case ProjectTeam.NorthEast:
                    return "North East";
                case ProjectTeam.SouthWest:
                    return "South West";
                case ProjectTeam.EastMidlands:
                    return "East Midlands";
                case ProjectTeam.RegionalCaseWorkerServices:
                    return "Regional casework services";
                case ProjectTeam.ServiceSupport:
                    return "Service support";
                case ProjectTeam.BusinessSupport:
                    return "Business support";
                case ProjectTeam.DataConsumers:
                    return "Data consumers";
                default:
                    throw new ArgumentOutOfRangeException(nameof(projectTeam), projectTeam, null);
            }
        }
    }
}
