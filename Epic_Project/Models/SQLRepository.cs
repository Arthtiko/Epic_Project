using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class SQLRepository : IRepository
    {
        private readonly IConfiguration _configuration;

        private string defConnectionString = ConnString.IdentityConnectionString;
        private string connectionString = ConnString.EpicDBConnectionString;

        private List<Team> TeamList = new List<Team>();
        private List<Module> ModuleList = new List<Module>();
        private List<EpicBaseLine> EpicBaseLineList = new List<EpicBaseLine>();
        private List<Measurement> MeasurementList = new List<Measurement>();
        private List<Employee> EmployeeList = new List<Employee>();
        private List<Parameter> ParameterList = new List<Parameter>();
        private List<Date> DateList = new List<Date>();


        public SQLRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            //connectionString = _configuration.GetConnectionString("EPICDBConnection");
            //defConnectionString = _configuration.GetConnectionString("DefaultConnection");
        }

        #region Insert

        public EpicBaseLine InsertEpicBaseLine(EpicBaseLine epicBaseLine, string userName, string ipAddress)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[ins_EpicBaseLine]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@EPICId", epicBaseLine.EPICId);
                    sqlCommand.Parameters.AddWithValue("@EPICName", epicBaseLine.EPICName);
                    sqlCommand.Parameters.AddWithValue("@ModuleId", GetModuleIdByName(epicBaseLine.ModuleName.ModuleName));
                    sqlCommand.Parameters.AddWithValue("@EpicType", GetParameterValue("EpicType", epicBaseLine.EpicType.TypeName));
                    sqlCommand.Parameters.AddWithValue("@ProjectLocation", GetParameterValue("ProjectLocation", epicBaseLine.ProjectLocation.LocationName));
                    sqlCommand.Parameters.AddWithValue("@Estimation", epicBaseLine.Estimation);
                    if (epicBaseLine.Team.TeamName == null)
                    {
                        epicBaseLine.Team.TeamName = "";
                        sqlCommand.Parameters.AddWithValue("@TeamId", 0);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@TeamId", GetTeamIdByName(epicBaseLine.Team.TeamName));
                    }
                    sqlCommand.Parameters.AddWithValue("@IsMurabaha", GetParameterValue("IsMurabaha", epicBaseLine.IsMurabaha.MurabahaName));
                    sqlCommand.Parameters.AddWithValue("@IsFirstSellableModule", GetParameterValue("IsFirstSellableModule", epicBaseLine.IsFirstSellableModule.FirstSellableModuleName));
                    sqlCommand.Parameters.AddWithValue("@DistributedUnmappedEffort", epicBaseLine.DistributedUnmappedEffort);
                    sqlCommand.Parameters.AddWithValue("@TotalActualEffort", epicBaseLine.TotalActualEffort);
                    sqlCommand.Parameters.AddWithValue("@UserName", userName);
                    sqlCommand.Parameters.AddWithValue("@UserIp", ipAddress);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return epicBaseLine;
        }

        public Measurement InsertMeasurement(Measurement measurement, string userName, string ipAddress)
        {
            List<EpicBaseLine> epic = (List<EpicBaseLine>)GetEpicBaseLineAll(measurement.EpicId);
            if (measurement.EpicId != epic[0].EPICId)
            {
                return null;
            }
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[ins_Measurement]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@EpicId", measurement.EpicId);
                    sqlCommand.Parameters.AddWithValue("@Year", measurement.Year);
                    sqlCommand.Parameters.AddWithValue("@Month", measurement.Month);
                    sqlCommand.Parameters.AddWithValue("@Type", GetParameterValue("Type", measurement.Type.TypeName));
                    sqlCommand.Parameters.AddWithValue("@RequirementProgress", (double)(measurement.RequirementProgress / 100.00));
                    sqlCommand.Parameters.AddWithValue("@DesignProgress", (double)(measurement.DesignProgress / 100.00));
                    sqlCommand.Parameters.AddWithValue("@DevelopmentProgress", (double)(measurement.DevelopmentProgress / 100.00));
                    sqlCommand.Parameters.AddWithValue("@TestProgress", (double)(measurement.TestProgress / 100.00));
                    sqlCommand.Parameters.AddWithValue("@UatProgress", (double)(measurement.UatProgress / 100.00));
                    sqlCommand.Parameters.AddWithValue("@PreviousMonthCumulativeActualEffort", measurement.PreviousMonthCumulativeActualEffort);
                    sqlCommand.Parameters.AddWithValue("@ActualEffort", measurement.ActualEffort);
                    sqlCommand.Parameters.AddWithValue("@UserName", userName);
                    sqlCommand.Parameters.AddWithValue("@UserIp", ipAddress);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return measurement;
        }

        public Module InsertModule(Module module)
        {
            List<Module> modules = (List<Module>)GetModuleAll();
            Module mod = modules.Find(m => m.ModuleName == module.ModuleName);
            if (mod != null)
            {
                return null;
            }
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[ins_Module]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@ModuleName", module.ModuleName);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return module;
        }

        public Team InsertTeam(Team team)
        {
            List<Team> teams = (List<Team>)GetTeamAll(0, team.TeamName, team.TeamLeader.TeamLeaderId, team.ProjectManager.ProjectManagerId);
            if (teams == null || teams.Count() >= 1)
            {
                return null;
            }
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[ins_Team]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@TeamName", team.TeamName);
                    sqlCommand.Parameters.AddWithValue("@TeamLeaderId", GetEmployeeIdByName(team.TeamLeader.TeamLeaderName));
                    sqlCommand.Parameters.AddWithValue("@ProjectManagerId", GetEmployeeIdByName(team.ProjectManager.ProjectManagerName));
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return team;
        }

        public Employee InsertEmployee(Employee employee)
        {
            List<Employee> employees = (List<Employee>)GetEmployeeAll();
            Employee emp = employees.Find(e => e.EmployeeName == employee.EmployeeName);
            if (emp != null)
            {
                return null;
            }
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[ins_Employee]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@EmployeeName", employee.EmployeeName);
                    sqlCommand.Parameters.AddWithValue("@EmployeeType", GetParameterValue("EmployeeType", employee.EmployeeType.TypeName));
                    sqlCommand.Parameters.AddWithValue("@EmployeeLocation", GetParameterValue("ProjectLocation", employee.EmployeeLocation.LocationName));
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return employee;
        }
        #endregion

        #region Delete

        public void DeleteEpicBaseLine(int id, string userName, string ipAddress)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[del_EpicBaseLine]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@EPICId", id);
                    sqlCommand.Parameters.AddWithValue("@UserName", userName);
                    sqlCommand.Parameters.AddWithValue("@UserIp", ipAddress);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
        }

        public void DeleteMeasurement(int epicId, int year, int month, int type, string userName, string ipAddress)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[del_Measurement]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@EpicId", epicId);
                    sqlCommand.Parameters.AddWithValue("@Year", year);
                    sqlCommand.Parameters.AddWithValue("@Month", month);
                    sqlCommand.Parameters.AddWithValue("@Type", type);
                    sqlCommand.Parameters.AddWithValue("@UserName", userName);
                    sqlCommand.Parameters.AddWithValue("@UserIp", ipAddress);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
        }

        public void DeleteModule(int id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[del_Module]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@ModuleId", id);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
        }

        public void DeleteTeam(int id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[del_Team]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@TeamId", id);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
        }

        public void DeleteEmployee(int id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[del_Employee]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@EmployeeId", id);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
        }
        #endregion

        #region Get

        public IEnumerable<EpicBaseLine> GetEpicBaseLineAll(int id)
        {
            EpicBaseLineList.Clear();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_EpicBaseLine]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    if ( id != 0 )
                    {
                        sqlCommand.Parameters.AddWithValue("@EPICId", id);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@EPICId", null);
                    }
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                EpicBaseLine temp = new EpicBaseLine();
                temp.EPICId = Convert.ToInt32(dt.Rows[i]["EPICId"]);
                temp.EPICName = Convert.ToString(dt.Rows[i]["EPICName"]);
                temp.ModuleName.ModuleId = Convert.ToInt32(dt.Rows[i]["ModuleId"]);
                temp.ModuleName.ModuleName = Convert.ToString(dt.Rows[i]["ModuleName"]);
                temp.EpicType.TypeValue = Convert.ToInt32(dt.Rows[i]["EpicTypeId"]);
                temp.EpicType.TypeName = Convert.ToString(dt.Rows[i]["EpicType"]);
                temp.ProjectLocation.LocationValue = Convert.ToInt32(dt.Rows[i]["ProjectLocationId"]);
                temp.ProjectLocation.LocationName = Convert.ToString(dt.Rows[i]["ProjectLocation"]);
                temp.Estimation = (float)Convert.ToDouble(dt.Rows[i]["Estimation"]);
                temp.EpicWeight = (float)Convert.ToDouble(dt.Rows[i]["EpicWeight"]);
                if (Convert.IsDBNull(dt.Rows[i]["TeamId"]))
                {
                    temp.Team = new Team()
                    {
                        TeamId = 0,
                        TeamName = "",
                        TeamLeader = new TeamLeaderViewModel()
                        {
                            TeamLeaderId = 0,
                            TeamLeaderName = ""
                        },
                        ProjectManager = new ProjectManagerViewModel()
                        {
                            ProjectManagerId = 0,
                            ProjectManagerName = ""
                        },
                        TeamLocation = temp.ProjectLocation.LocationName
                    };
                }
                else
                {
                    temp.Team.TeamId = Convert.ToInt32(dt.Rows[i]["TeamId"]);
                    temp.Team.TeamName = Convert.ToString(dt.Rows[i]["TeamName"]);
                    temp.Team.ProjectManager = new ProjectManagerViewModel()
                    {
                        ProjectManagerId = Convert.ToInt32(dt.Rows[i]["ProjectManagerId"]),
                        ProjectManagerName = Convert.ToString(dt.Rows[i]["ProjectManagerName"])
                    };
                    temp.Team.TeamLeader = new TeamLeaderViewModel()
                    {
                        TeamLeaderId = Convert.ToInt32(dt.Rows[i]["TeamLeaderId"]),
                        TeamLeaderName = Convert.ToString(dt.Rows[i]["TeamLeaderName"])
                    };
                }
                temp.IsMurabaha.MurabahaValue = Convert.ToInt32(dt.Rows[i]["IsMurabahaId"]);
                temp.IsMurabaha.MurabahaName = Convert.ToString(dt.Rows[i]["IsMurabaha"]);
                temp.IsFirstSellableModule.FirstSellableModuleValue = Convert.ToInt32(dt.Rows[i]["IsFirstSellableModuleId"]);
                temp.IsFirstSellableModule.FirstSellableModuleName = Convert.ToString(dt.Rows[i]["IsFirstSellableModule"]);
                temp.DistributedUnmappedEffort = (float)Convert.ToDouble(dt.Rows[i]["DistributedUnmappedEffort"]);
                temp.ActualEffort = (float)Convert.ToDouble(dt.Rows[i]["ActualEffort"]);
                temp.TotalActualEffort = (float)Convert.ToDouble(dt.Rows[i]["TotalActualEffort"]);
                EpicBaseLineList.Add(temp);
            }
            return EpicBaseLineList;
        }
        
        public IEnumerable<Measurement> GetMeasurementAll(int epicId, int year, int month, string type)
        {
            MeasurementList = new List<Measurement>();
            DataTable dt = new DataTable();
            
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Measurement]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    if (epicId != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@EpicId", epicId);
                    }
                    if (year != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@Year", year);
                    }
                    if (month != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@Month", month);
                    }
                    if (type != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@Type", GetParameterValue("Type", type));
                    }
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Measurement temp = new Measurement();
                temp.EpicId = Convert.ToInt32(dt.Rows[i]["EpicId"]);
                temp.EpicName = Convert.ToString(dt.Rows[i]["EpicName"]);
                temp.Module.ModuleId = Convert.ToInt32(dt.Rows[i]["ModuleId"]);
                temp.Module.ModuleName = Convert.ToString(dt.Rows[i]["ModuleName"]);
                temp.Year = Convert.ToInt32(dt.Rows[i]["Year"]);
                temp.Month = Convert.ToInt32(dt.Rows[i]["Month"]);
                temp.Type.TypeValue = Convert.ToInt32(dt.Rows[i]["TypeId"]);
                temp.Type.TypeName = Convert.ToString(dt.Rows[i]["TypeName"]);
                temp.EpicWeight = (float)Convert.ToDouble(dt.Rows[i]["EpicWeight"]);
                if (Convert.IsDBNull(dt.Rows[i]["TeamId"]))
                {
                    temp.Team.TeamId = 0;
                    temp.Team.TeamName = "";
                    temp.Team.TeamLeader = new TeamLeaderViewModel()
                    {
                        TeamLeaderId = 0,
                        TeamLeaderName = ""
                    };
                    temp.Team.ProjectManager = new ProjectManagerViewModel()
                    {
                        ProjectManagerId = 0,
                        ProjectManagerName = ""
                    };
                }
                else
                {
                    temp.Team.TeamId = Convert.ToInt32(dt.Rows[i]["TeamId"]);
                    temp.Team.TeamName = Convert.ToString(dt.Rows[i]["TeamName"]);
                    temp.Team.ProjectManager = new ProjectManagerViewModel()
                    {
                        ProjectManagerId = Convert.ToInt32(dt.Rows[i]["ProjectManagerId"]),
                        ProjectManagerName = Convert.ToString(dt.Rows[i]["ProjectManagerName"])
                    };
                    temp.Team.TeamLeader = new TeamLeaderViewModel()
                    {
                        TeamLeaderId = Convert.ToInt32(dt.Rows[i]["TeamLeaderId"]),
                        TeamLeaderName = Convert.ToString(dt.Rows[i]["TeamLeaderName"])
                    };
                }
                temp.IsFirstSellableModule = Convert.ToString(dt.Rows[i]["IsFirstSellableModule"]);
                temp.RequirementProgress = (float)Convert.ToDouble(dt.Rows[i]["RequirementProgress"]);
                temp.DesignProgress = (float)Convert.ToDouble(dt.Rows[i]["DesignProgress"]);
                temp.DevelopmentProgress = (float)Convert.ToDouble(dt.Rows[i]["DevelopmentProgress"]);
                temp.TestProgress = (float)Convert.ToDouble(dt.Rows[i]["TestProgress"]);
                temp.UatProgress = (float)Convert.ToDouble(dt.Rows[i]["UatProgress"]);
                temp.OverallEpicCompilation = (float)Convert.ToDouble(dt.Rows[i]["OverallEpicCompilation"]);
                temp.WeightedOverallProgress = (float)Convert.ToDouble(dt.Rows[i]["WeightedOverallProgress"]);
                temp.PreviousMonthCumulativeActualEffort = (float)Convert.ToDouble(dt.Rows[i]["PreviousMonthCumulativeActualEffort"]);
                temp.ActualEffort = (float)Convert.ToDouble(dt.Rows[i]["ActualEffort"]);
                MeasurementList.Add(temp);
            }
            return MeasurementList;
        }
        
        public IEnumerable<MeasurementLog> GetMeasurementLogs(int epicId, int year, int month, string type)
        {
            List<MeasurementLog> logs = new List<MeasurementLog>();
            DataTable dt = new DataTable();

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Measurement_Logs]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    if (epicId != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@EpicId", epicId);
                    }
                    if (year != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@Year", year);
                    }
                    if (month != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@Month", month);
                    }
                    if (type != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@Type", GetParameterValue("Type", type));
                    }
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                MeasurementLog temp = new MeasurementLog();
                temp.EpicId = Convert.ToInt32(dt.Rows[i]["EpicId"]);
                temp.EpicName = Convert.ToString(dt.Rows[i]["EpicName"]);
                temp.Module = Convert.ToString(dt.Rows[i]["ModuleName"]);
                temp.Year = Convert.ToInt32(dt.Rows[i]["Year"]);
                temp.Month = Convert.ToInt32(dt.Rows[i]["Month"]);
                temp.Type = Convert.ToString(dt.Rows[i]["TypeName"]);
                temp.Team = Convert.ToString(dt.Rows[i]["TeamName"]);
                temp.RequirementProgress = (float)Convert.ToDouble(dt.Rows[i]["RequirementProgress"]);
                temp.DesignProgress = (float)Convert.ToDouble(dt.Rows[i]["DesignProgress"]);
                temp.DevelopmentProgress = (float)Convert.ToDouble(dt.Rows[i]["DevelopmentProgress"]);
                temp.TestProgress = (float)Convert.ToDouble(dt.Rows[i]["TestProgress"]);
                temp.UatProgress = (float)Convert.ToDouble(dt.Rows[i]["UatProgress"]);
                temp.PreviousMonthCumulativeActualEffort = (float)Convert.ToDouble(dt.Rows[i]["PreviousMonthCumulativeActualEffort"]);
                temp.ActualEffort = (float)Convert.ToDouble(dt.Rows[i]["ActualEffort"]);
                temp.UserName = Convert.ToString(dt.Rows[i]["UserName"]);
                temp.Time = (Convert.ToDateTime(dt.Rows[i]["Time"]).ToLocalTime()).ToString("dd/MM/yyyy HH:mm");
                temp.UserIp = Convert.ToString(dt.Rows[i]["UserIp"]);
                logs.Add(temp);
            }
            return logs;
        }

        public IEnumerable<Module> GetModuleAll()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Module]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Module temp = new Module();
                temp.ModuleId = Convert.ToInt32(dt.Rows[i]["ModuleId"]);
                temp.ModuleName = Convert.ToString(dt.Rows[i]["ModuleName"]);
                temp.Progress = 0;
                temp.Weight = 0;
                ModuleList.Add(temp);
            }
            return ModuleList;
        }
        
        public IEnumerable<Team> GetTeamAll(int id, string name, int teamLeaderId, int projectManagerId)
        {
            List<Employee> empList = (List<Employee>)GetEmployeeAll();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Team]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    if (id != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@TeamId", id);
                    }
                    if (name != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@TeamName", name);
                    }
                    if (teamLeaderId != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@TeamLeaderId", teamLeaderId);
                    }
                    if (projectManagerId != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@ProjectManagerId", projectManagerId);
                    }
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Team temp = new Team();
                temp.TeamId = Convert.ToInt32(dt.Rows[i]["TeamId"]);
                temp.TeamName = Convert.ToString(dt.Rows[i]["TeamName"]);
                temp.TeamLeader.TeamLeaderId = Convert.ToInt32(dt.Rows[i]["TeamLeaderId"]);
                temp.TeamLeader.TeamLeaderName = Convert.ToString(dt.Rows[i]["TeamLeader"]);
                temp.ProjectManager.ProjectManagerId = Convert.ToInt32(dt.Rows[i]["ProjectManagerId"]);
                temp.ProjectManager.ProjectManagerName = Convert.ToString(dt.Rows[i]["ProjectManager"]);
                temp.TeamLocation = Convert.ToString(dt.Rows[i]["TeamLocationName"]);
                TeamList.Add(temp);
            }
            return TeamList;
        }
        
        public IEnumerable<Employee> GetEmployeeAll()
        {
            EmployeeList.Clear();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Employee]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Employee temp = new Employee();
                temp.EmployeeId = Convert.ToInt32(dt.Rows[i]["EmployeeId"]);
                temp.EmployeeName = Convert.ToString(dt.Rows[i]["EmployeeName"]);
                temp.EmployeeType.TypeName = Convert.ToString(dt.Rows[i]["EmployeeType"]);
                temp.EmployeeType.TypeId = Convert.ToInt32(dt.Rows[i]["EmployeeTypeId"]);
                temp.EmployeeLocation.LocationName = Convert.ToString(dt.Rows[i]["EmployeeLocation"]);
                temp.EmployeeLocation.LocationValue = Convert.ToInt32(dt.Rows[i]["EmployeeLocationId"]);
                EmployeeList.Add(temp);
            }
            return EmployeeList;
        }

        public IEnumerable<Employee> GetEmployeeByType(int type)
        {
            EmployeeList.Clear();
            List<Employee> emps = new List<Employee>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Employee]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@EmployeeType", type);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Employee temp = new Employee();
                temp.EmployeeId = Convert.ToInt32(dt.Rows[i]["EmployeeId"]);
                temp.EmployeeName = Convert.ToString(dt.Rows[i]["EmployeeName"]);
                temp.EmployeeType.TypeName = Convert.ToString(dt.Rows[i]["EmployeeType"]);
                temp.EmployeeType.TypeId = Convert.ToInt32(dt.Rows[i]["EmployeeTypeId"]);
                temp.EmployeeLocation.LocationName = Convert.ToString(dt.Rows[0]["EmployeeLocation"]);
                EmployeeList.Add(temp);
            }
            if (type == 3)
            {
                emps = (List<Employee>)GetEmployeeByType(1);
            }
            if (emps != null && emps.Count() > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Employee temp = emps[i];
                    EmployeeList.Add(temp);
                }
            }
            return EmployeeList;
        }

        public Employee GetEmployeeById(int id)
        {
            EmployeeList.Clear();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Employee]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@EmployeeId", id);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            Employee employee = new Employee();
            employee.EmployeeId = Convert.ToInt32(dt.Rows[0]["EmployeeId"]);
            employee.EmployeeName = Convert.ToString(dt.Rows[0]["EmployeeName"]);
            employee.EmployeeType.TypeName = Convert.ToString(dt.Rows[0]["EmployeeType"]);
            employee.EmployeeType.TypeId = Convert.ToInt32(dt.Rows[0]["EmployeeTypeId"]);
            employee.EmployeeLocation.LocationName = Convert.ToString(dt.Rows[0]["EmployeeLocation"]);
            return employee;
        }
        
        public IEnumerable<Parameter> GetParameter(string name)
        {
            ParameterList.Clear();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Parameter]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@ColumnName", name);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Parameter temp = new Parameter();
                temp.ParameterValue = Convert.ToInt32(dt.Rows[i]["ParameterValue"]);
                temp.ParameterName = Convert.ToString(dt.Rows[i]["ParameterName"]);
                ParameterList.Add(temp);
            }
            return ParameterList;
        }

        public int GetParameterValue(string columnName, string parameterName)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Parameter]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@ColumnName", columnName);
                    sqlCommand.Parameters.AddWithValue("@ParameterName", parameterName);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            int value = Convert.ToInt32(dt.Rows[0]["ParameterValue"]);
            return value;
        }

        #endregion

        #region Update

        public Employee UpdateEmployee(Employee updatedEmployee)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[upd_Employee]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@EmployeeId", updatedEmployee.EmployeeId);
                    sqlCommand.Parameters.AddWithValue("@EmployeeName", updatedEmployee.EmployeeName);
                    sqlCommand.Parameters.AddWithValue("@EmployeeType", GetParameterValue("EmployeeType", updatedEmployee.EmployeeType.TypeName));
                    sqlCommand.Parameters.AddWithValue("@EmployeeLocation", GetParameterValue("ProjectLocation", updatedEmployee.EmployeeLocation.LocationName));
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return updatedEmployee;
        }

        public EpicBaseLine UpdateEpicBaseLine(EpicBaseLine updatedEpicBaseLine, string userName, string ipAddress)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[upd_EpicBaseLine]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@EPICId", updatedEpicBaseLine.EPICId);
                    sqlCommand.Parameters.AddWithValue("@EPICName", updatedEpicBaseLine.EPICName);
                    sqlCommand.Parameters.AddWithValue("@ModuleId", updatedEpicBaseLine.ModuleName.ModuleId);
                    sqlCommand.Parameters.AddWithValue("@EpicType", GetParameterValue("EpicType", updatedEpicBaseLine.EpicType.TypeName));
                    sqlCommand.Parameters.AddWithValue("@ProjectLocation", GetParameterValue("ProjectLocation", updatedEpicBaseLine.ProjectLocation.LocationName));
                    sqlCommand.Parameters.AddWithValue("@Estimation", updatedEpicBaseLine.Estimation);
                    sqlCommand.Parameters.AddWithValue("@TeamId", updatedEpicBaseLine.Team.TeamId);
                    sqlCommand.Parameters.AddWithValue("@IsMurabaha", GetParameterValue("IsMurabaha", updatedEpicBaseLine.IsMurabaha.MurabahaName));
                    sqlCommand.Parameters.AddWithValue("@IsFirstSellableModule", GetParameterValue("IsFirstSellableModule", updatedEpicBaseLine.IsFirstSellableModule.FirstSellableModuleName));
                    sqlCommand.Parameters.AddWithValue("@DistributedUnmappedEffort", updatedEpicBaseLine.DistributedUnmappedEffort);
                    sqlCommand.Parameters.AddWithValue("@TotalActualEffort", updatedEpicBaseLine.TotalActualEffort);
                    sqlCommand.Parameters.AddWithValue("@UserName", userName);
                    sqlCommand.Parameters.AddWithValue("@UserIp", ipAddress);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return updatedEpicBaseLine;

        }

        public Measurement UpdateMeasurement(Measurement updatedMeasurement, string userName, string ipAddress)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[upd_Measurement]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@EpicId", updatedMeasurement.EpicId);
                    sqlCommand.Parameters.AddWithValue("@Year", updatedMeasurement.Year);
                    sqlCommand.Parameters.AddWithValue("@Month", updatedMeasurement.Month);
                    sqlCommand.Parameters.AddWithValue("@Type", GetParameterValue("Type", updatedMeasurement.Type.TypeName));
                    sqlCommand.Parameters.AddWithValue("@RequirementProgress", updatedMeasurement.RequirementProgress / 100);
                    sqlCommand.Parameters.AddWithValue("@DesignProgress", updatedMeasurement.DesignProgress / 100);
                    sqlCommand.Parameters.AddWithValue("@DevelopmentProgress", updatedMeasurement.DevelopmentProgress / 100);
                    sqlCommand.Parameters.AddWithValue("@TestProgress", updatedMeasurement.TestProgress / 100);
                    sqlCommand.Parameters.AddWithValue("@UatProgress", updatedMeasurement.UatProgress / 100);
                    sqlCommand.Parameters.AddWithValue("@PreviousMonthCumulativeActualEffort", updatedMeasurement.PreviousMonthCumulativeActualEffort);
                    sqlCommand.Parameters.AddWithValue("@ActualEffort", updatedMeasurement.ActualEffort);
                    sqlCommand.Parameters.AddWithValue("@UserName", userName);
                    sqlCommand.Parameters.AddWithValue("@UserIp", ipAddress);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return updatedMeasurement;
        }

        public Module UpdateModule(Module updatedModule)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[upd_Module]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@ModuleId", updatedModule.ModuleId);
                    sqlCommand.Parameters.AddWithValue("@ModuleName", updatedModule.ModuleName);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return updatedModule;
        }

        public Team UpdateTeam(Team updatedTeam)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[upd_Team]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@TeamId", updatedTeam.TeamId);
                    sqlCommand.Parameters.AddWithValue("@TeamName", updatedTeam.TeamName);
                    sqlCommand.Parameters.AddWithValue("@TeamLeaderId", updatedTeam.TeamLeader.TeamLeaderId);
                    sqlCommand.Parameters.AddWithValue("@ProjectManagerId", updatedTeam.ProjectManager.ProjectManagerId);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return updatedTeam;
        }
        #endregion

        #region Self
        public List<int> GetEpicBaseLineIdByLocation(string location)
        {
            List<int> list = new List<int>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_EpicBaseLine]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@ProjectLocation", GetParameterValue("ProjectLocation", location));
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(Convert.ToInt32(dt.Rows[i]["EPICId"]));
            }
            return list;
        }
        public List<int> GetEpicBaseLineIdByTeam(string teamName)
        {
            List<int> list = new List<int>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_EpicBaseLine]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    if (teamName == null || teamName == "")
                    {
                        sqlCommand.Parameters.AddWithValue("@TeamId", 0);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@TeamId", GetTeamIdByName(teamName));
                    }
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                list.Add(Convert.ToInt32(dt.Rows[i]["EPICId"]));
            }
            return list;
        }
        public int GetModuleIdByName(string name)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Module]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@ModuleName", name);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            Module module = new Module();
            module.ModuleId = Convert.ToInt32(dt.Rows[0]["ModuleId"]);
            return module.ModuleId;
        }
        public int GetTeamIdByName(string name)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Team]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@TeamName", name);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            Team team = new Team();
            if (dt.Rows.Count > 0)
            {
                team.TeamId = Convert.ToInt32(dt.Rows[0]["TeamId"]);
                return team.TeamId;
            }
            return 0;
        }
        public int GetEmployeeIdByName(string name)
        {
            EmployeeList.Clear();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Employee]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@EmployeeName", name);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            Employee employee = new Employee();
            employee.EmployeeId = Convert.ToInt32(dt.Rows[0]["EmployeeId"]);
            employee.EmployeeName = Convert.ToString(dt.Rows[0]["EmployeeName"]);
            employee.EmployeeType.TypeName = Convert.ToString(dt.Rows[0]["EmployeeType"]);
            employee.EmployeeType.TypeId = GetParameterValue("EmployeeType", employee.EmployeeType.TypeName);
            //employee.EmployeeLocation = Convert.ToString(dt.Rows[0]["EmployeeLocation"]);
            return employee.EmployeeId;
        }
        #endregion

        #region Date Control

        public IEnumerable<DateControl> GetDateControl(int? year, int? month, string type)
        {
            List<DateControl> DateControlList = new List<DateControl>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_DateControl]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Year", year);
                    sqlCommand.Parameters.AddWithValue("@Month", month);
                    if (type != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@Type", GetParameterValue("DateControlType", type));
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@Type", type);
                    }
                    
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                DateControl temp = new DateControl();
                temp.Year = Convert.ToInt32(dt.Rows[i]["Year"]);
                temp.Month = Convert.ToInt32(dt.Rows[i]["Month"]);
                temp.DateControlType.TypeId = Convert.ToInt32(dt.Rows[i]["TypeId"]);
                temp.DateControlType.TypeName = Convert.ToString(dt.Rows[i]["TypeName"]);
                temp.Effort.EffortId = Convert.ToInt32(dt.Rows[i]["EffortId"]);
                temp.Effort.EffortName = Convert.ToString(dt.Rows[i]["EffortName"]);
                temp.Progress.ProgressId = Convert.ToInt32(dt.Rows[i]["ProgressId"]);
                temp.Progress.ProgressName = Convert.ToString(dt.Rows[i]["ProgressName"]);
                temp.Variance.VarianceId = Convert.ToInt32(dt.Rows[i]["VarianceId"]);
                temp.Variance.VarianceName = Convert.ToString(dt.Rows[i]["VarianceName"]);
                DateControlList.Add(temp);
            }
            return DateControlList;
        }

        public DateControl InsertDateControl(DateControl dateControl)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[ins_DateControl]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Year", dateControl.Year);
                    sqlCommand.Parameters.AddWithValue("@Month", dateControl.Month);
                    sqlCommand.Parameters.AddWithValue("@Effort", dateControl.Effort.EffortId);
                    sqlCommand.Parameters.AddWithValue("@Progress", dateControl.Progress.ProgressId);
                    sqlCommand.Parameters.AddWithValue("@Variance", dateControl.Variance.VarianceId);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return dateControl;
        }

        public void DeleteDateControl(int year, int month)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[del_DateControl]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Year", year);
                    sqlCommand.Parameters.AddWithValue("@Month", month);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
        }

        public DateControl UpdateDateControl(DateControl dateControl)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[upd_DateControl]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Year", dateControl.Year);
                    sqlCommand.Parameters.AddWithValue("@Month", dateControl.Month);
                    sqlCommand.Parameters.AddWithValue("@Type", dateControl.DateControlType.TypeId);
                    sqlCommand.Parameters.AddWithValue("@Effort", dateControl.Effort.EffortId);
                    sqlCommand.Parameters.AddWithValue("@Progress", dateControl.Progress.ProgressId);
                    sqlCommand.Parameters.AddWithValue("@Variance", dateControl.Variance.VarianceId);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return dateControl;
        }
        #endregion

        #region Finance

        public IEnumerable<Finance> GetFinanceAll(string category, int year, int month)
        {
            List<Finance> financeList = new List<Finance>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Finance]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    if (category != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@Category", category);
                    }
                    if (year != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@Year", year);
                    }
                    if (month != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@Month", month);
                    }
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Finance temp = new Finance();
                temp.Category = Convert.ToString(dt.Rows[i]["Category"]);
                temp.Year = Convert.ToInt32(dt.Rows[i]["Year"]);
                temp.Month = Convert.ToInt32(dt.Rows[i]["Month"]);
                temp.PeriodBudget = (float)Convert.ToDouble(dt.Rows[i]["PeriodBudget"]);
                temp.TotalBudget = (float)Convert.ToDouble(dt.Rows[i]["TotalBudget"]);
                temp.Actual = (float)Convert.ToDouble(dt.Rows[i]["Actual"]);
                financeList.Add(temp);
            }
            return financeList;
        }
        public IEnumerable<FinanceReport> GetFinanceReport(string category, int year, int month)
        {
            List<FinanceReport> financeList = new List<FinanceReport>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_FinanceReport]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    if (category != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@Category", category);
                    }
                    if (year != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@Year", year);
                    }
                    if (month != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@Month", month);
                    }
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                FinanceReport temp = new FinanceReport();
                temp.Category = Convert.ToString(dt.Rows[i]["Category"]);
                temp.Year = Convert.ToInt32(dt.Rows[i]["Year"]);
                temp.Month = Convert.ToInt32(dt.Rows[i]["Month"]);
                temp.PeriodBudget = (float)Convert.ToDouble(dt.Rows[i]["PeriodBudget"]);
                temp.Actual = (float)Convert.ToDouble(dt.Rows[i]["Actual"]);
                temp.PeriodActualPercentage = (float)Convert.ToDouble(dt.Rows[i]["PeriodActualPercentage"]);
                temp.TotalBudget = (float)Convert.ToDouble(dt.Rows[i]["TotalBudget"]);
                temp.TotalRemaining = (float)Convert.ToDouble(dt.Rows[i]["TotalRemaining"]);
                temp.TotalActualPercentage = (float)Convert.ToDouble(dt.Rows[i]["TotalActualPercentage"]);
                financeList.Add(temp);
            }
            return financeList;
        }
        public Finance InsertFinanceAll(Finance finance)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[ins_Finance]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Category", finance.Category);
                    sqlCommand.Parameters.AddWithValue("@Year", finance.Year);
                    sqlCommand.Parameters.AddWithValue("@Month", finance.Month);
                    sqlCommand.Parameters.AddWithValue("@PeriodBudget", finance.PeriodBudget);
                    sqlCommand.Parameters.AddWithValue("@TotalBudget", finance.TotalBudget);
                    sqlCommand.Parameters.AddWithValue("@Actual", finance.Actual);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return finance;
        }
        public void DeleteFinance(Finance finance)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[del_Finance]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Category", finance.Category);
                    sqlCommand.Parameters.AddWithValue("@Year", finance.Year);
                    sqlCommand.Parameters.AddWithValue("@Month", finance.Month);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
        }
        public Finance UpdateFinanceAll(Finance finance)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[upd_Finance]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Category", finance.Category);
                    sqlCommand.Parameters.AddWithValue("@Year", finance.Year);
                    sqlCommand.Parameters.AddWithValue("@Month", finance.Month);
                    sqlCommand.Parameters.AddWithValue("@PeriodBudget", finance.PeriodBudget);
                    sqlCommand.Parameters.AddWithValue("@TotalBudget", finance.TotalBudget);
                    sqlCommand.Parameters.AddWithValue("@Actual", finance.Actual);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return finance;
        }
        public List<Date> GetFinanceDates()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_financeDates]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Date temp = new Date();
                temp.Year = Convert.ToInt32(dt.Rows[i]["Year"]);
                temp.Month = Convert.ToInt32(dt.Rows[i]["Month"]);
                DateList.Add(temp);
            }
            return DateList;
        }
        public void GenerateNewFinanceMonth()
        {
            Date date = GetFinanceDates()[0];
            Date newDate = new Date()
            {
                Month = date.Month == 12 ? 1 : date.Month + 1,
                Year = date.Month == 12 ? date.Year + 1 : date.Year
            };

            List<Finance> finances = (List<Finance>)GetFinanceAll(null, date.Year, date.Month);

            for (int i = 0; i < finances.Count(); i++)
            {
                Finance temp = finances[i];
                temp.Year = newDate.Year;
                temp.Month = newDate.Month;
                InsertFinanceAll(temp);
            }
        }
        public void DeleteLastFinanceMonth()
        {
            Date date = GetFinanceDates()[0];
            Finance finance = new Finance() { Year = date.Year, Month = date.Month};

            DeleteFinance(finance);
        }
        public FinanceGraph GetFinanceGraphData(int year, int month, bool isTotal)
        {
            string cat1 = "\tArchtecht - Technical & Businessteam Cost";
            string cat2 = "\tITS - Technical & Business team Cost";
            string cat3 = "\tLicense fee to KTP";
            string cat4 = "\tServers";
            string cat5 = "\tTravel Expenses - Archtecht";
            string cat6 = "\tTravel Expenses - ITS";
            FinanceGraph temp = new FinanceGraph();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_FinanceGraph]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Category", null);
                    if (year != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@Year", year);
                    }
                    if (month != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@Month", month);
                    }
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string tempCategory = Convert.ToString(dt.Rows[i]["Category"]);
                if (tempCategory == cat1)
                {
                    if (isTotal)
                    {
                        temp.Line1 = (float)Convert.ToDouble(dt.Rows[i]["TotalActual"]);
                    }
                    else
                    {
                        temp.Line1 = (float)Convert.ToDouble(dt.Rows[i]["PeriodActual"]);
                    }
                }
                if (tempCategory == cat2)
                {
                    if (isTotal)
                    {
                        temp.Line2 = (float)Convert.ToDouble(dt.Rows[i]["TotalActual"]);
                    }
                    else
                    {
                        temp.Line2 = (float)Convert.ToDouble(dt.Rows[i]["PeriodActual"]);
                    }
                }
                if (tempCategory == cat3)
                {
                    if (isTotal)
                    {
                        temp.Line3 = (float)Convert.ToDouble(dt.Rows[i]["TotalActual"]);
                    }
                    else
                    {
                        temp.Line3 = (float)Convert.ToDouble(dt.Rows[i]["PeriodActual"]);
                    }
                }
                if (tempCategory == cat4)
                {
                    if (isTotal)
                    {
                        temp.Line4 = (float)Convert.ToDouble(dt.Rows[i]["TotalActual"]);
                    }
                    else
                    {
                        temp.Line4 = (float)Convert.ToDouble(dt.Rows[i]["PeriodActual"]);
                    }
                }
                if (tempCategory == cat5)
                {
                    if (isTotal)
                    {
                        temp.Line5 = (float)Convert.ToDouble(dt.Rows[i]["TotalActual"]);
                    }
                    else
                    {
                        temp.Line5 = (float)Convert.ToDouble(dt.Rows[i]["PeriodActual"]);
                    }
                }
                if (tempCategory == cat6)
                {
                    if (isTotal)
                    {
                        temp.Line6 = (float)Convert.ToDouble(dt.Rows[i]["TotalActual"]);
                    }
                    else
                    {
                        temp.Line6 = (float)Convert.ToDouble(dt.Rows[i]["PeriodActual"]);
                    }
                }
                temp.Category = Convert.ToString(dt.Rows[i]["Year"]) + " - " + Convert.ToString(dt.Rows[i]["Month"]);
            }
            return temp;
        }
        public IEnumerable<FinanceGraph> GetFinanceGraph(bool isTotal)
        {
            List<Date> dates = GetFinanceDates();

            List<FinanceGraph> financeList = new List<FinanceGraph>();
            
            for (int i = dates.Count() - 1; i >= 0; i--)
            {
                FinanceGraph temp = GetFinanceGraphData(dates[i].Year, dates[i].Month, isTotal);
                financeList.Add(temp);
            }

            return financeList;
        }
        #endregion

        public List<MeasurementDetailsViewModel> FillMeasurementDetails(int year, int month)
        {
            int prevYear = month == 1 ? year - 1 : year;
            int prevMonth = month == 1 ? 12 : month - 1;
            List<MeasurementDetailsViewModel> MeasurementDetailsList = new List<MeasurementDetailsViewModel>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_MeasurementReport]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Year", year);
                    sqlCommand.Parameters.AddWithValue("@PrevYear", prevYear);
                    sqlCommand.Parameters.AddWithValue("@Month", month);
                    sqlCommand.Parameters.AddWithValue("@PrevMonth", prevMonth);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                MeasurementDetailsViewModel temp = new MeasurementDetailsViewModel();
                temp.EpicId = Convert.ToInt32(dt.Rows[i]["EpicId"]);
                temp.EpicName = Convert.ToString(dt.Rows[i]["EpicName"]);
                temp.Module.ModuleName = Convert.ToString(dt.Rows[i]["ModuleName"]);
                temp.Year = Convert.ToInt32(dt.Rows[i]["Year"]);
                temp.Month = Convert.ToInt32(dt.Rows[i]["Month"]);
                temp.Location = Convert.ToString(dt.Rows[i]["ProjectLocation"]);
                temp.EpicWeight = (float)Convert.ToDouble(dt.Rows[i]["EpicWeight"]);
                temp.Estimation = (float)Convert.ToDouble(dt.Rows[i]["Estimation"]);
                if (Convert.IsDBNull(dt.Rows[i]["TeamName"]))
                {
                    temp.Team.TeamName = "";
                }
                else
                {
                    temp.Team.TeamName = Convert.ToString(dt.Rows[i]["TeamName"]);
                }
                temp.IsFirstSellableModule = Convert.ToString(dt.Rows[i]["IsFirstSellableModule"]);
                if (Convert.IsDBNull(dt.Rows[i]["PreviousMonthRequirementProgress"]))
                {
                    temp.PrevMonthRequirementProgress = 0;
                    temp.PrevMonthDesignProgress = 0;
                    temp.PrevMonthDevelopmentProgress = 0;
                    temp.PrevMonthTestProgress = 0;
                    temp.PrevMonthUatProgress = 0;
                    temp.PrevMonthOverallEpicCompilation = 0;
                    temp.PrevMonthWeightedOverallProgress = 0;
                }
                else
                {
                    temp.PrevMonthRequirementProgress = (float)Convert.ToDouble(dt.Rows[i]["PreviousMonthRequirementProgress"]);
                    temp.PrevMonthDesignProgress = (float)Convert.ToDouble(dt.Rows[i]["PreviousMonthDesignProgress"]);
                    temp.PrevMonthDevelopmentProgress = (float)Convert.ToDouble(dt.Rows[i]["PreviousMonthDevelopmentProgress"]);
                    temp.PrevMonthTestProgress = (float)Convert.ToDouble(dt.Rows[i]["PreviousMonthTestProgress"]);
                    temp.PrevMonthUatProgress = (float)Convert.ToDouble(dt.Rows[i]["PreviousMonthUatProgress"]);
                    temp.PrevMonthOverallEpicCompilation = (float)Convert.ToDouble(dt.Rows[i]["PreviousMonthOverallEpicCompilation"]);
                    temp.PrevMonthWeightedOverallProgress = (float)Convert.ToDouble(dt.Rows[i]["PreviousMonthWeightedOverallProgress"]);
                }
                if (Convert.IsDBNull(dt.Rows[i]["ActualRequirementProgress"]))
                {
                    temp.ActualRequirementProgress = 0;
                    temp.ActualDesignProgress = 0;
                    temp.ActualDevelopmentProgress = 0;
                    temp.ActualTestProgress = 0;
                    temp.ActualUatProgress = 0;
                    temp.ActualOverallEpicCompilation = 0;
                    temp.ActualWeightedOverallProgress = 0;
                }
                else
                {
                    temp.ActualRequirementProgress = (float)Convert.ToDouble(dt.Rows[i]["ActualRequirementProgress"]);
                    temp.ActualDesignProgress = (float)Convert.ToDouble(dt.Rows[i]["ActualDesignProgress"]);
                    temp.ActualDevelopmentProgress = (float)Convert.ToDouble(dt.Rows[i]["ActualDevelopmentProgress"]);
                    temp.ActualTestProgress = (float)Convert.ToDouble(dt.Rows[i]["ActualTestProgress"]);
                    temp.ActualUatProgress = (float)Convert.ToDouble(dt.Rows[i]["ActualUatProgress"]);
                    temp.ActualOverallEpicCompilation = (float)Convert.ToDouble(dt.Rows[i]["ActualOverallEpicCompilation"]);
                    temp.ActualWeightedOverallProgress = (float)Convert.ToDouble(dt.Rows[i]["ActualWeightedOverallProgress"]);
                }
                if (Convert.IsDBNull(dt.Rows[i]["ActualRequirementProgress"]))
                {
                    temp.TargetRequirementProgress = 0;
                    temp.TargetDesignProgress = 0;
                    temp.TargetDevelopmentProgress = 0;
                    temp.TargetTestProgress = 0;
                    temp.TargetUatProgress = 0;
                    temp.TargetOverallEpicCompilation = 0;
                    temp.TargetWeightedOverallProgress = 0;
                }
                else
                {
                    temp.TargetRequirementProgress = (float)Convert.ToDouble(dt.Rows[i]["TargetRequirementProgress"]);
                    temp.TargetDesignProgress = (float)Convert.ToDouble(dt.Rows[i]["TargetDesignProgress"]);
                    temp.TargetDevelopmentProgress = (float)Convert.ToDouble(dt.Rows[i]["TargetDevelopmentProgress"]);
                    temp.TargetTestProgress = (float)Convert.ToDouble(dt.Rows[i]["TargetTestProgress"]);
                    temp.TargetUatProgress = (float)Convert.ToDouble(dt.Rows[i]["TargetUatProgress"]);
                    temp.TargetOverallEpicCompilation = (float)Convert.ToDouble(dt.Rows[i]["TargetOverallEpicCompilation"]);
                    temp.TargetWeightedOverallProgress = (float)Convert.ToDouble(dt.Rows[i]["TargetWeightedOverallProgress"]);
                }
                temp.PreviousMonthCumulativeActualEffort = (float)Convert.ToDouble(dt.Rows[i]["PreviousMonthCumulativeActualEffort"]);
                temp.ActualEffort = (float)Convert.ToDouble(dt.Rows[i]["ActualEffort"]);
                temp.Variance = (float)Convert.ToDouble(dt.Rows[i]["Variance"]);

                MeasurementDetailsList.Add(temp);
            }


            return MeasurementDetailsList;
        }

        public List<Measurement> GenerateMeasurementForNextMonth(int year, int month, string location, string userName, string ipAddress)
        {
            int prevYear = (month == 1 ? year - 1 : year);
            int prevMonth = (month == 1 ? 12 : month - 1);
            List<Measurement> CurrentMeasurementList = (List<Measurement>)GetMeasurementAll(0, prevYear, prevMonth, null);
            List<Measurement> TempMeasurementList = new List<Measurement>();
            List<Measurement> NextMonthMeasurementList = CurrentMeasurementList;
            for (int i = 0; i < CurrentMeasurementList.Count(); i++)
            {
                Measurement temp = new Measurement()
                {
                    EpicId = CurrentMeasurementList[i].EpicId,
                    Year = year,
                    Month = month,
                    Type = CurrentMeasurementList[i].Type,
                    RequirementProgress = CurrentMeasurementList[i].RequirementProgress,
                    DesignProgress = CurrentMeasurementList[i].DesignProgress,
                    DevelopmentProgress = CurrentMeasurementList[i].DevelopmentProgress,
                    TestProgress = CurrentMeasurementList[i].TestProgress,
                    UatProgress = CurrentMeasurementList[i].UatProgress,
                    PreviousMonthCumulativeActualEffort = CurrentMeasurementList[i].PreviousMonthCumulativeActualEffort + CurrentMeasurementList[i].ActualEffort,
                    ActualEffort = 0
                };
                TempMeasurementList.Add(temp);
            }
            for (int i = 0; i < TempMeasurementList.Count(); i++)
            {
                InsertMeasurement(TempMeasurementList[i], userName, ipAddress);
                NextMonthMeasurementList.Add(TempMeasurementList[i]);
            }
            return NextMonthMeasurementList;
        }

        public List<Date> GetDates()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_dates]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Date temp = new Date();
                temp.Year = Convert.ToInt32(dt.Rows[i]["Year"]);
                temp.Month = Convert.ToInt32(dt.Rows[i]["Month"]);
                DateList.Add(temp);
            }
            return DateList;
        }
        
        public void DeleteLastMonth(int month, int year, string location, string userName, string ipAddress)
        {
            List<Date> dates = GetDates();
            int prevYear = dates[0].Year;
            int prevMonth = dates[0].Month;
            List<Measurement> LastmonthMeasurement = (List<Measurement>)GetMeasurementAll(0, prevYear, prevMonth, null);
            for (int i = 0; i < LastmonthMeasurement.Count(); i++)
            {
                DeleteMeasurement(LastmonthMeasurement[i].EpicId, prevYear, prevMonth, LastmonthMeasurement[i].Type.TypeValue, userName, ipAddress);
            }
        }
        
        public List<Measurement> SearchMeasurement(int year, int month, string location, string type, string teamName)
        {
            List<Measurement> list = new List<Measurement>();
            List<int> IdListByLocation = GetEpicBaseLineIdByLocation(location);
            List<int> IdListByTeam = GetEpicBaseLineIdByTeam(teamName);
            List<Measurement> measurements = (List<Measurement>)GetMeasurementAll(0, year, month, type);
            for (int i = 0; i < measurements.Count(); i++)
            {
                int c1 = IdListByLocation.Find(id => id == measurements[i].EpicId);
                if (teamName == null)
                {
                    if (c1 != 0)
                    {
                        list.Add(measurements[i]);
                    }
                }
                else
                {
                    int c2 = IdListByTeam.Find(id => id == measurements[i].EpicId);
                    if (c1 != 0 && c2 != 0)
                    {
                        list.Add(measurements[i]);
                    }
                }
                
            }
            return list;
        }

        public int GetEmployeeId(string userId)
        {
            int empId = 0;
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(defConnectionString))
            {
                string procName = "[sel_EmployeeId]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Id", userId);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
                if (!Convert.IsDBNull(dt.Rows))
                {
                    empId = Convert.ToInt32(dt.Rows[0]["EmployeeId"]); 
                }
            }
            return empId;
        }

        public ProgressModel GetProgress(int year, int month, string location, string isFirstSellableModule)
        {
            ProgressModel model = new ProgressModel() { ActualEffort = 0, Completed = 0, Variance = 0, Total = 0};
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_CompletedProgress]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Year", year);
                    sqlCommand.Parameters.AddWithValue("@Month", month);
                    if (location != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@Location", GetParameterValue("ProjectLocation", location));
                    }
                    if (isFirstSellableModule != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@IsFirstSellableModule", GetParameterValue("IsFirstSellableModule", isFirstSellableModule));
                    }
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            if (dt.Rows.Count <= 0)
            {
                return null;
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                model.Completed = model.Completed + (float)Convert.ToDouble(dt.Rows[i]["OverallCompilation"]);
                model.ActualEffort = model.ActualEffort + (float)Convert.ToDouble(dt.Rows[i]["ActualEffort"]);
                model.Variance = model.Variance + (float)Convert.ToDouble(dt.Rows[i]["Variance"]);
            }
            return model;
        }

        public float GetEpicWeight(string location, string isFirstSellableModule)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_EpicWeight]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    if (location != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@Location", GetParameterValue("ProjectLocation", location));
                    }
                    if (isFirstSellableModule != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@IsFirstSellableModule", GetParameterValue("IsFirstSellableModule", isFirstSellableModule));
                    }
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            float sum = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sum = sum + (float)Convert.ToDouble(dt.Rows[i]["EpicWeight"]);
            }
            return sum;
        }

        public List<Module> GetModuleProgress(int year, int month)
        {
            List<Module> modules = (List<Module>)GetModuleAll();
            year = month == 1 ? year - 1 : year;
            month = month == 1 ? 12 : month - 1;
            List<Module> moduleAggregates = GetModuleAggregates(year, month);
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_ModuleProgress]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Year", year);
                    sqlCommand.Parameters.AddWithValue("@Month", month);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Module temp = new Module();
                temp.ModuleId = Convert.ToInt32(dt.Rows[i]["ModuleId"]);
                temp.ModuleName = Convert.ToString(dt.Rows[i]["ModuleName"]);
                temp.Progress = (float)Convert.ToDouble(dt.Rows[i]["OverallCompilation"]);
                temp.Weight = (float)Convert.ToDouble(dt.Rows[i]["EpicWeight"]);
                temp.Variance = (float)Convert.ToDouble(dt.Rows[i]["Variance"]);
                temp.ActualEffort = (float)Convert.ToDouble(dt.Rows[i]["ActualEffort"]);
                temp.WeightedOverallProgress = (float)Convert.ToDouble(dt.Rows[i]["OverallCompilation"]);
                for (int j = 0; j < modules.Count(); j++)
                {
                    if (modules[j].ModuleId == temp.ModuleId)
                    {
                        modules[j].WeightedOverallProgress = modules[j].WeightedOverallProgress + temp.WeightedOverallProgress;
                        modules[j].Progress = modules[j].Progress + temp.Progress;
                        modules[j].Weight = modules[j].Weight + temp.Weight;
                        modules[j].Variance = modules[j].Variance + temp.Variance;
                        modules[j].ActualEffort = modules[j].ActualEffort + temp.ActualEffort;
                    }
                }
            }
            for (int i = 0; i < modules.Count(); i++)
            {
                modules[i].Progress = (float)Math.Round(modules[i].Progress / modules[i].Weight, 2);
            }
            List<Module> SortedList = modules.OrderByDescending(mo => mo.Progress).ToList();
            for (int i = 0; i < SortedList.Count(); i++)
            {
                for (int j = 0; j < moduleAggregates.Count(); j++)
                {
                    if (SortedList[i].ModuleId == moduleAggregates[j].ModuleId)
                    {
                        SortedList[i].EpicCount = moduleAggregates[j].EpicCount;
                        SortedList[i].TotalEstimation = moduleAggregates[j].TotalEstimation;
                    }
                }
            }
            return SortedList;
        }

        public List<Module> GetModuleAggregates(int year, int month)
        {
            List<Module> moduleList = new List<Module>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_ModuleAggregate]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    if (year != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@Year", year);
                    }
                    if (month != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@Month", month);
                    }
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Module temp = new Module();
                temp.ModuleId = Convert.ToInt32(dt.Rows[i]["ModuleId"]);
                temp.EpicCount = Convert.ToInt32(dt.Rows[i]["EpicCount"]);
                temp.TotalEstimation = (float)Convert.ToDouble(dt.Rows[i]["TotalEstimation"]);
                moduleList.Add(temp);
            }
            return moduleList;
        }

        public IEnumerable<LineChartModel> GetLineChartProgress(string location, string isFirstSellableModule, string type)
        {
            List<Date> dates = GetDates();
            List<DateControl> dateControlList = (List<DateControl>)GetDateControl(null, null, type);
            List<LineChartModel> models = new List<LineChartModel>();
            List<ProgressModel> tempModels = new List<ProgressModel>();

            LineChartModel tempModel = new LineChartModel();

            for (int i = 0; i < dates.Count(); i++)
            {
                tempModels.Add(GetProgress(dates[i].Year, dates[i].Month, location, isFirstSellableModule));
            }
            
            for (int i = tempModels.Count() - 1; i >= 0; i--)
            {
                tempModel = new LineChartModel();
                if (dateControlList[i].Effort.EffortName == "TRUE")
                {
                    tempModel.ActualEffort = tempModels[i].ActualEffort;
                }
                else
                {
                    tempModel.ActualEffort = null;
                }
                if (dateControlList[i].Progress.ProgressName == "TRUE")
                {
                    tempModel.OverallCompilation = tempModels[i].Completed;
                }
                else
                {
                    tempModel.OverallCompilation = null;
                }
                if (dateControlList[i].Variance.VarianceName == "TRUE")
                {
                    tempModel.Variance = tempModels[i].Variance;
                }
                else
                {
                    tempModel.Variance = null;
                }
                tempModel.Category = dates[i].Year + " - " + dates[i].Month;
                models.Add(tempModel);
            }
            //models.Last().ActualEffort = null;
            //models.Last().Variance = null;

            return models;
        }

        public IEnumerable<HighLevelProgress> GetHighLevelProgress(string location, string isFirstSellableModule, Date date)
        {
            List<HighLevelProgress> highLevelProgresses = new List<HighLevelProgress>();
            HighLevelProgress requirement = new HighLevelProgress();
            HighLevelProgress design = new HighLevelProgress();
            HighLevelProgress development = new HighLevelProgress();
            HighLevelProgress test = new HighLevelProgress();
            HighLevelProgress uat = new HighLevelProgress();

            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_HighLevelProgress]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Year", date.Year);
                    sqlCommand.Parameters.AddWithValue("@Month", date.Month);
                    sqlCommand.Parameters.AddWithValue("@Type", 2);
                    if (location != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@Location", GetParameterValue("ProjectLocation", location));
                    }
                    if (isFirstSellableModule != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@IsFirstSellableModule", GetParameterValue("IsFirstSellableModule", isFirstSellableModule));
                    }
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
            {
                highLevelProgresses = new List<HighLevelProgress>();
                requirement = new HighLevelProgress()
                {
                    TaskBreakdown = "Requirement",
                    InProgress = Convert.ToInt32(dt.Rows[0]["RequirementInProgress"]),
                    InQueue = Convert.ToInt32(dt.Rows[0]["RequirementInQueue"]),
                    Finished = Convert.ToInt32(dt.Rows[0]["RequirementFinished"]),
                    Total = Convert.ToInt32(dt.Rows[0]["Total"])
                };
                highLevelProgresses.Add(requirement);

                design = new HighLevelProgress()
                {
                    TaskBreakdown = "Design",
                    InProgress = Convert.ToInt32(dt.Rows[0]["DesignInProgress"]),
                    InQueue = Convert.ToInt32(dt.Rows[0]["DesignInQueue"]),
                    Finished = Convert.ToInt32(dt.Rows[0]["DesignFinished"]),
                    Total = Convert.ToInt32(dt.Rows[0]["Total"])
                };
                highLevelProgresses.Add(design);

                development = new HighLevelProgress()
                {
                    TaskBreakdown = "Development",
                    InProgress = Convert.ToInt32(dt.Rows[0]["DevelopmentInProgress"]),
                    InQueue = Convert.ToInt32(dt.Rows[0]["DevelopmentInQueue"]),
                    Finished = Convert.ToInt32(dt.Rows[0]["DevelopmentFinished"]),
                    Total = Convert.ToInt32(dt.Rows[0]["Total"])
                };
                highLevelProgresses.Add(development);

                test = new HighLevelProgress()
                {
                    TaskBreakdown = "Test",
                    InProgress = Convert.ToInt32(dt.Rows[0]["TestInProgress"]),
                    InQueue = Convert.ToInt32(dt.Rows[0]["TestInQueue"]),
                    Finished = Convert.ToInt32(dt.Rows[0]["TestFinished"]),
                    Total = Convert.ToInt32(dt.Rows[0]["Total"])
                };
                highLevelProgresses.Add(test);

                uat = new HighLevelProgress()
                {
                    TaskBreakdown = "UAT",
                    InProgress = Convert.ToInt32(dt.Rows[0]["UatInProgress"]),
                    InQueue = Convert.ToInt32(dt.Rows[0]["UatInQueue"]),
                    Finished = Convert.ToInt32(dt.Rows[0]["UatFinished"]),
                    Total = Convert.ToInt32(dt.Rows[0]["Total"])
                };
                highLevelProgresses.Add(uat);
            }

            return highLevelProgresses;
        }

    }
}
