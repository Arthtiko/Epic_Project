using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class SQLRepository : IRepository
    {
        private string defConnectionString = GetIdentityConnectionString();
        private string connectionString = GetEPICDBConnectionString();

        private List<Team> TeamList = new List<Team>();
        private List<Module> ModuleList = new List<Module>();
        private List<EpicBaseLine> EpicBaseLineList = new List<EpicBaseLine>();
        private List<Measurement> MeasurementList = new List<Measurement>();
        private List<Employee> EmployeeList = new List<Employee>();
        private List<Parameter> ParameterList = new List<Parameter>();
        private List<Date> DateList = new List<Date>();

        public SQLRepository()
        {

        }


        public static string GetEPICDBConnectionString()
        {
            return Startup.EPICDBConnectionString;
        }
        public static string GetIdentityConnectionString()
        {
            return Startup.IdentityConnectionString;
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
                    sqlCommand.Parameters.AddWithValue("@EditMode", epicBaseLine.EditMode.Name);
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
                    sqlCommand.Parameters.AddWithValue("@RequirementProgress", Math.Round(measurement.RequirementProgress / 100.00, 2));
                    sqlCommand.Parameters.AddWithValue("@DesignProgress", Math.Round(measurement.DesignProgress / 100.00, 2));
                    sqlCommand.Parameters.AddWithValue("@DevelopmentProgress", Math.Round(measurement.DevelopmentProgress / 100.00, 2));
                    sqlCommand.Parameters.AddWithValue("@TestProgress", Math.Round(measurement.TestProgress / 100.00, 2));
                    sqlCommand.Parameters.AddWithValue("@UatProgress", Math.Round(measurement.UatProgress / 100.00, 2));
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

        public void DeleteMeasurementLog(int epicId, int year, int month, int type)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[del_Measurement_Log]";
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
                    if (type != 0)
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
                temp.FSMPercentage = (float)Convert.ToDouble(dt.Rows[i]["FSMPercentage"]);
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
                temp.Description = Convert.ToString(dt.Rows[i]["Description"]);
                temp.Dependency = Convert.ToString(dt.Rows[i]["Dependency"]);
                string mode = Convert.ToString(dt.Rows[i]["EditMode"]);
                temp.EditMode.Value = mode == "Epic" ? 1 : mode == "Feature" ? 2 : 0;
                temp.EditMode.Name = mode == "Epic" || mode == "Feature" ? mode : "";
                EpicBaseLineList.Add(temp);
            }
            return EpicBaseLineList;
        }
        
        public IEnumerable<Measurement> GetMeasurementAll(int epicId, int year, int month, string type, int teamId)
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
                    if (teamId != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@TeamId", teamId);
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
                temp.FSMPercentage = (float)Convert.ToDouble(dt.Rows[i]["FSMPercentage"]);
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
                string m = Convert.ToString(dt.Rows[i]["EditMode"]);
                temp.EditMode = new EditModeModel() { Value = m == "Epic" ? 1 : m == "Feature" ? 2 : 0, Name = m == "Epic" || m == "Feature" ? m : "" }; 
                MeasurementList.Add(temp);
            }
            return MeasurementList;
        }

        public IEnumerable<MeasurementLogModel> GetLogMeasurement(int epicId, int year, int month, string type, string user)
        {
            List<EpicBaseLine> epics = (List<EpicBaseLine>)GetEpicBaseLineAll(0);
            List<MeasurementLogModel> logList = new List<MeasurementLogModel>();
            List<MeasurementLog> longLogList = new List<MeasurementLog>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Measurement_Logs]";
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
                    if (type == null)
                    {
                        sqlCommand.Parameters.AddWithValue("@Type", null);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@Type", GetParameterValue("Type", type));
                    }
                    if (user == null)
                    {
                        sqlCommand.Parameters.AddWithValue("@UserName", null);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@UserName", user);
                    }
                    if (epicId != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@EpicId", epicId);
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
                if (Convert.ToString(dt.Rows[i]["UserName"]) != "Egypt Test Admin" && Convert.ToString(dt.Rows[i]["UserName"]) != "Turkey Test Admin")
                {
                    MeasurementLogModel temp = new MeasurementLogModel();
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
                    if (!Convert.IsDBNull(dt.Rows[i]["UserName"]))
                    {
                        temp.UserName = Convert.ToString(dt.Rows[i]["UserName"]);
                    }
                    temp.Time = Convert.ToDateTime(dt.Rows[i]["Time"]).ToLocalTime().ToString("dd/MM/yyyy HH:mm");
                    //if (!Convert.IsDBNull(dt.Rows[i]["UserIp"]))
                    //{
                    //    temp.UserIp = Convert.ToString(dt.Rows[i]["UserIp"]);
                    //}
                    logList.Add(temp);
                }
            }
            for (int j = 0; j < epics.Count(); j++)
            {
                List<MeasurementLogModel> modelList = logList.FindAll(l => l.EpicId == epics[j].EPICId && l.Type == "Target");
                for (int k = 0; k < modelList.Count() -1; k++)
                {
                    MeasurementLog temp = new MeasurementLog();
                    temp.EpicId = modelList[k].EpicId;
                    temp.Year = modelList[k].EpicId;
                    temp.Month = modelList[k].Month;
                    temp.Type = modelList[k].Type;
                    temp.NewValues = new MeasurementValues();
                    temp.NewValues.RequirementProgress = modelList[k].RequirementProgress;
                    temp.NewValues.DesignProgress = modelList[k].DesignProgress;
                    temp.NewValues.DevelopmentProgress = modelList[k].DevelopmentProgress;
                    temp.NewValues.TestProgress = modelList[k].TestProgress;
                    temp.NewValues.UatProgress = modelList[k].UatProgress;
                    temp.NewValues.PreviousMonthCumulativeActualEffort = modelList[k].PreviousMonthCumulativeActualEffort;
                    temp.OldValues = new MeasurementValues();
                    temp.OldValues.ActualEffort = modelList[k].ActualEffort;
                    temp.OldValues.RequirementProgress = modelList[k + 1].RequirementProgress;
                    temp.OldValues.DesignProgress = modelList[k + 1].DesignProgress;
                    temp.OldValues.DevelopmentProgress = modelList[k + 1].DevelopmentProgress;
                    temp.OldValues.TestProgress = modelList[k + 1].TestProgress;
                    temp.OldValues.UatProgress = modelList[k + 1].UatProgress;
                    temp.OldValues.PreviousMonthCumulativeActualEffort = modelList[k + 1].PreviousMonthCumulativeActualEffort;
                    temp.OldValues.ActualEffort = modelList[k + 1].ActualEffort;
                    temp.UserName = modelList[k].UserName;
                    temp.Time = modelList[k].Time;
                    longLogList.Add(temp);
                }
                modelList = logList.FindAll(l => l.EpicId == epics[j].EPICId && l.Type == "Actual");
                for (int k = 0; k < modelList.Count()-1; k++)
                {
                    MeasurementLog temp = new MeasurementLog();
                    temp.EpicId = modelList[k].EpicId;
                    temp.Year = modelList[k].EpicId;
                    temp.Month = modelList[k].Month;
                    temp.Type = modelList[k].Type;
                    temp.NewValues = new MeasurementValues();
                    temp.NewValues.RequirementProgress = modelList[k].RequirementProgress;
                    temp.NewValues.DesignProgress = modelList[k].DesignProgress;
                    temp.NewValues.DevelopmentProgress = modelList[k].DevelopmentProgress;
                    temp.NewValues.TestProgress = modelList[k].TestProgress;
                    temp.NewValues.UatProgress = modelList[k].UatProgress;
                    temp.NewValues.PreviousMonthCumulativeActualEffort = modelList[k].PreviousMonthCumulativeActualEffort;
                    temp.NewValues.ActualEffort = modelList[k].ActualEffort;
                    temp.OldValues = new MeasurementValues();
                    temp.OldValues.RequirementProgress = modelList[k + 1].RequirementProgress;
                    temp.OldValues.DesignProgress = modelList[k + 1].DesignProgress;
                    temp.OldValues.DevelopmentProgress = modelList[k + 1].DevelopmentProgress;
                    temp.OldValues.TestProgress = modelList[k + 1].TestProgress;
                    temp.OldValues.UatProgress = modelList[k + 1].UatProgress;
                    temp.OldValues.PreviousMonthCumulativeActualEffort = modelList[k + 1].PreviousMonthCumulativeActualEffort;
                    temp.OldValues.ActualEffort = modelList[k + 1].ActualEffort;
                    temp.UserName = modelList[k].UserName;
                    temp.Time = modelList[k].Time;
                    longLogList.Add(temp);
                }
            }
            
            return logList;
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
                    sqlCommand.Parameters.AddWithValue("@FSMPercentage", Math.Round(updatedEpicBaseLine.FSMPercentage, 2));
                    sqlCommand.Parameters.AddWithValue("@IsMurabaha", GetParameterValue("IsMurabaha", updatedEpicBaseLine.IsMurabaha.MurabahaName));
                    sqlCommand.Parameters.AddWithValue("@IsFirstSellableModule", GetParameterValue("IsFirstSellableModule", updatedEpicBaseLine.IsFirstSellableModule.FirstSellableModuleName));
                    sqlCommand.Parameters.AddWithValue("@DistributedUnmappedEffort", updatedEpicBaseLine.DistributedUnmappedEffort);
                    sqlCommand.Parameters.AddWithValue("@TotalActualEffort", updatedEpicBaseLine.TotalActualEffort);
                    if (updatedEpicBaseLine.Description != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@Description", updatedEpicBaseLine.Description);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@Description", "");
                    }
                    if (updatedEpicBaseLine.Dependency != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@Dependency", updatedEpicBaseLine.Dependency);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@Dependency", "");
                    }
                    sqlCommand.Parameters.AddWithValue("@UserName", userName);
                    sqlCommand.Parameters.AddWithValue("@UserIp", ipAddress);
                    sqlCommand.Parameters.AddWithValue("@EditMode", updatedEpicBaseLine.EditMode.Name);
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
                    sqlCommand.Parameters.AddWithValue("@RequirementProgress", Math.Round(updatedMeasurement.RequirementProgress / 100, 4));
                    sqlCommand.Parameters.AddWithValue("@DesignProgress", Math.Round(updatedMeasurement.DesignProgress / 100, 4));
                    sqlCommand.Parameters.AddWithValue("@DevelopmentProgress", Math.Round(updatedMeasurement.DevelopmentProgress / 100, 4));
                    sqlCommand.Parameters.AddWithValue("@TestProgress", Math.Round(updatedMeasurement.TestProgress / 100, 4));
                    sqlCommand.Parameters.AddWithValue("@UatProgress", Math.Round(updatedMeasurement.UatProgress / 100, 4));
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
            string cat7 = "Migration Activity Ethix NG";
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
                if (tempCategory == cat7)
                {
                    if (isTotal)
                    {
                        temp.Line7 = (float)Convert.ToDouble(dt.Rows[i]["TotalActual"]);
                    }
                    else
                    {
                        temp.Line7 = (float)Convert.ToDouble(dt.Rows[i]["PeriodActual"]);
                    }
                }
                temp.Category = Convert.ToString(dt.Rows[i]["Year"]) + " - " + Convert.ToString(dt.Rows[i]["Month"]);
            }
            return temp;
        }
        public List<FinanceGraph> GetFinanceTotalValues(int year, int month)
        {
            List<FinanceGraph> list = new List<FinanceGraph>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_FinanceGraphTotal]";
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
                FinanceGraph temp = new FinanceGraph();
                temp.Category = "Total";
                temp.Line1 = (float)Convert.ToDouble(dt.Rows[i]["TotalPercentage"]);
                temp.Line2 = (float)Convert.ToDouble(dt.Rows[i]["PeriodPercentage"]);
                list.Add(temp);
            }
            return list;
        }
        public IEnumerable<FinanceGraph> GetFinanceGraph(bool isTotal)
        {
            List<Date> dates = GetFinanceDates();

            List<FinanceGraph> financeList = new List<FinanceGraph>();
            
            for (int i = dates.Count() - 1; i >= 0; i--)
            {
                FinanceGraph temp = GetFinanceGraphData(dates[i].Year, dates[i].Month, isTotal);
                FinanceGraph totals = GetFinanceTotalValues(dates[i].Year, dates[i].Month)[0];
                if (isTotal)
                {
                    temp.Line8 = totals.Line1;
                }
                else
                {
                    temp.Line8 = totals.Line2;
                }
                financeList.Add(temp);
            }

            return financeList;
        }
        #endregion

        #region VarianceAnalysis

        public IEnumerable<ProgressProducingVarianceAnalysis> GetProducingVarianceAnalysis(int teamId, int year, int month)
        {
            List<ProgressProducingVarianceAnalysis> varList = new List<ProgressProducingVarianceAnalysis>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_VarianceAnalysis]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    if (teamId != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@TeamId", teamId);
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
                ProgressProducingVarianceAnalysis temp = new ProgressProducingVarianceAnalysis();
                temp.Team = new Team()
                {
                    TeamId = Convert.ToInt32(dt.Rows[i]["TeamId"]),
                    TeamName = Convert.ToString(dt.Rows[i]["TeamName"])
                };
                temp.Year = Convert.ToInt32(dt.Rows[i]["Year"]);
                temp.Month = Convert.ToInt32(dt.Rows[i]["Month"]);
                temp.ResourceCount = Convert.ToInt32(dt.Rows[i]["ProgressProducingResourceCount"]);
                temp.DayOff = Convert.ToInt32(dt.Rows[i]["ProgressProducingResourceDayOff"]);
                temp.TargetProgress = (float)Convert.ToDouble(dt.Rows[i]["TargetProgress"]);

                temp.PlannedManday = temp.ResourceCount * 22 - temp.DayOff;
                temp.PlannedConsumedMandayBudget = (temp.PlannedManday / GetTotalEstimation()) * 100;
                temp.ThresholdIncrementProgress = (float)(temp.PlannedConsumedMandayBudget * 1.3);
                int prevYear = temp.Month == 1 ? temp.Year - 1 : temp.Year;
                int prevMonth = temp.Month == 1 ? 12 : temp.Month - 1;
                List<Measurement> ms = (List<Measurement>)GetMeasurementAll(0, prevYear, prevMonth, "Actual", temp.Team.TeamId);
                float prevMonthProgress = 0;
                for (int j = 0; j < ms.Count(); j++)
                {
                    prevMonthProgress += ms[j].WeightedOverallProgress;
                }
                temp.PreviousMonthOverallProgress = prevMonthProgress;
                temp.IncrementProgress = temp.TargetProgress - temp.PreviousMonthOverallProgress;
                temp.Difference = temp.ThresholdIncrementProgress - temp.IncrementProgress;
                temp.Variance = temp.PlannedConsumedMandayBudget - temp.IncrementProgress;
                
                varList.Add(temp);
            }
            return varList;
        }

        public IEnumerable<NonProgressProducingVarianceAnalysis> GetNonProducingVarianceAnalysis(int teamId, int year, int month)
        {
            List<NonProgressProducingVarianceAnalysis> varList = new List<NonProgressProducingVarianceAnalysis>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_VarianceAnalysis]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    if (teamId != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@TeamId", teamId);
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
                NonProgressProducingVarianceAnalysis temp = new NonProgressProducingVarianceAnalysis();
                temp.Team = new Team()
                {
                    TeamId = Convert.ToInt32(dt.Rows[i]["TeamId"]),
                    TeamName = Convert.ToString(dt.Rows[i]["TeamName"])
                };
                temp.Year = Convert.ToInt32(dt.Rows[i]["Year"]);
                temp.Month = Convert.ToInt32(dt.Rows[i]["Month"]);
                temp.NonProgressProducingResourceCount = Convert.ToInt32(dt.Rows[i]["NonProgressProducingResourceCount"]);
                temp.ResourceCount = Convert.ToInt32(dt.Rows[i]["ProgressProducingResourceCount"]);
                temp.TotalResourceCount = temp.ResourceCount + temp.NonProgressProducingResourceCount;

                temp.DayOff = Convert.ToInt32(dt.Rows[i]["NonProgressProducingResourceDayOff"]);
                temp.TargetProgress = (float)Convert.ToDouble(dt.Rows[i]["TargetProgress"]);

                temp.PlannedManday = temp.TotalResourceCount * 22 - temp.DayOff;
                temp.PlannedConsumedMandayBudget = (temp.PlannedManday / GetTotalEstimation()) * 100;
                temp.ThresholdIncrementProgress = (float)(temp.PlannedConsumedMandayBudget * 1.3);
                int prevYear = temp.Month == 1 ? temp.Year - 1 : temp.Year;
                int prevMonth = temp.Month == 1 ? 12 : temp.Month - 1;
                List<Measurement> ms = (List<Measurement>)GetMeasurementAll(0, prevYear, prevMonth, "Actual", temp.Team.TeamId);
                float prevMonthProgress = 0;
                for (int j = 0; j < ms.Count(); j++)
                {
                    prevMonthProgress += ms[j].WeightedOverallProgress;
                }
                temp.PreviousMonthOverallProgress = prevMonthProgress;
                temp.IncrementProgress = temp.TargetProgress - temp.PreviousMonthOverallProgress;
                temp.Difference = temp.ThresholdIncrementProgress - temp.IncrementProgress;
                temp.Variance = temp.PlannedConsumedMandayBudget - temp.IncrementProgress;

                varList.Add(temp);
            }
            return varList;
        }

        public void InsertVarianceAnalysis(VarianceAnalysis var)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[ins_VarianceAnalysis]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@TeamId", var.TeamId);
                    sqlCommand.Parameters.AddWithValue("@Year", var.Year);
                    sqlCommand.Parameters.AddWithValue("@Month", var.Month);
                    sqlCommand.Parameters.AddWithValue("@ProgressProducingResourceCount", var.ProgressProducingResourceCount);
                    sqlCommand.Parameters.AddWithValue("@NonProgressProducingResourceCount", var.NonProgressProducingResourceCount);
                    sqlCommand.Parameters.AddWithValue("@ProgressProducingResourceDayOff", var.ProgressProducingResourceDayOff);
                    sqlCommand.Parameters.AddWithValue("@NonProgressProducingResourceDayOff", var.NonProgressProducingResourceDayOff);
                    sqlCommand.Parameters.AddWithValue("@TargetProgress", Math.Round(var.TargetProgress, 2));
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
        }
        public void DeleteVarianceAnalysis(int teamId, int year, int month)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[del_VarianceAnalysis]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@TeamId", teamId);
                    sqlCommand.Parameters.AddWithValue("@Year", year);
                    sqlCommand.Parameters.AddWithValue("@Month",month);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
        }
        public void UpdateVarianceAnalysis(VarianceAnalysis var)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[upd_VarianceAnalysis]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@TeamId", var.TeamId);
                    sqlCommand.Parameters.AddWithValue("@Year", var.Year);
                    sqlCommand.Parameters.AddWithValue("@Month", var.Month);
                    sqlCommand.Parameters.AddWithValue("@ProgressProducingResourceCount", var.ProgressProducingResourceCount);
                    sqlCommand.Parameters.AddWithValue("@NonProgressProducingResourceCount", var.NonProgressProducingResourceCount);
                    sqlCommand.Parameters.AddWithValue("@ProgressProducingResourceDayOff", var.ProgressProducingResourceDayOff);
                    sqlCommand.Parameters.AddWithValue("@NonProgressProducingResourceDayOff", var.NonProgressProducingResourceDayOff);
                    sqlCommand.Parameters.AddWithValue("@TargetProgress", var.TargetProgress);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
        }

        public void InsertVarianceAnalysisDate(Date date)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[ins_VarianceAnalysisDate]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Year", date.Year);
                    sqlCommand.Parameters.AddWithValue("@Month", date.Month);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
        }
        public void DeleteVarianceAnalysisDate(Date date)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[del_VarianceAnalysisDate]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Year", date.Year);
                    sqlCommand.Parameters.AddWithValue("@Month", date.Month);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
        }
        public List<Date> GetVarianceAnalysisDates()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_VarianceAnalysisDate]";
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
        #endregion

        #region TimeSheet

        public IEnumerable<TimeSheet> GetTimeSheetAll(string name, string project, string task, int year, int month)
        {
            int nextYear = month == 12 ? year + 1 : year;
            int nextMonth = month == 12 ? 1 : month + 1;
            DateTime date = new DateTime(year, month, 1);
            DateTime nextDate = new DateTime(nextYear, nextMonth, 1);

            List<TimeSheet> timeSheets = new List<TimeSheet>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_TimeSheet]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    if (name != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@Name", name);
                    }
                    if (project != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@Project", project);
                    }
                    if (task != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@Task", task);
                    }
                    if (year != 0 && month != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@FirstDate", date);
                        sqlCommand.Parameters.AddWithValue("@LastDate", nextDate);
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
                TimeSheet temp = new TimeSheet();
                temp.Id = Convert.ToInt32(dt.Rows[i]["Id"]);
                temp.Name = Convert.ToString(dt.Rows[i]["Name"]);
                temp.Project = Convert.ToString(dt.Rows[i]["Project"]);
                temp.Task = Convert.ToString(dt.Rows[i]["Task"]);
                temp.Hour = (float)Convert.ToDouble(dt.Rows[i]["Hour"]);
                temp.Date = Convert.ToDateTime(dt.Rows[i]["Date"]);
                timeSheets.Add(temp);
            }
            return timeSheets;
        }

        public void InsertTimeSheetAll(TimeSheet timeSheet)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[ins_TimeSheet]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Name", timeSheet.Name);
                    sqlCommand.Parameters.AddWithValue("@Project", timeSheet.Project);
                    sqlCommand.Parameters.AddWithValue("@Task", timeSheet.Task);
                    sqlCommand.Parameters.AddWithValue("@Hour", timeSheet.Hour);
                    sqlCommand.Parameters.AddWithValue("@Date", timeSheet.Date);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
        }

        public void UpdateTimeSheet(TimeSheet timeSheet)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[upd_TimeSheet]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Id", timeSheet.Id);
                    sqlCommand.Parameters.AddWithValue("@Name", timeSheet.Name);
                    sqlCommand.Parameters.AddWithValue("@Project", timeSheet.Project);
                    sqlCommand.Parameters.AddWithValue("@Task", timeSheet.Task);
                    sqlCommand.Parameters.AddWithValue("@Hour", timeSheet.Hour);
                    sqlCommand.Parameters.AddWithValue("@Date", timeSheet.Date);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
        }

        public void DeleteTimeSheetAll(TimeSheet timeSheet)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[del_TimeSheet]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Id", timeSheet.Id);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
        }

        public IEnumerable<string> GetTimeSheetName()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_TimeSheetName]";
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
            List<string> names = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                names.Add(Convert.ToString(dt.Rows[i]["Name"]));
            }
            return names;
        }

        #endregion

        #region Feature

        public IEnumerable<Feature> GetFeatureAll(int featureId, int featureIsFSM, int epicId, int year, int month, int type, int teamId)
        {
            List<Feature> FeatureList = new List<Feature>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Feature]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    if (featureId != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@FeatureId", featureId);
                    }
                    if (featureIsFSM != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@FeatureIsFSM", featureIsFSM);
                    }
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
                    if (type != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@Type", type);
                    }
                    if (teamId != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@TeamId", teamId);
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
                Feature temp = new Feature();
                temp.FeatureId = Convert.ToInt32(dt.Rows[i]["FeatureId"]);
                temp.FeatureName = Convert.ToString(dt.Rows[i]["FeatureName"]);
                temp.FeatureEstimation = Convert.ToInt32(dt.Rows[i]["FeatureEstimation"]);
                temp.FSM.FSMValue = Convert.ToInt32(dt.Rows[i]["FeatureIsFSMValue"]);
                temp.FSM.FSMName = Convert.ToString(dt.Rows[i]["FeatureIsFSMName"]);
                temp.EpicId = Convert.ToInt32(dt.Rows[i]["EpicId"]);
                temp.Year = Convert.ToInt32(dt.Rows[i]["Year"]);
                temp.Month = Convert.ToInt32(dt.Rows[i]["Month"]);
                temp.TypeValue = Convert.ToInt32(dt.Rows[i]["TypeValue"]);
                temp.TypeName = Convert.ToString(dt.Rows[i]["TypeName"]);
                temp.RequirementProgress = (float)Convert.ToDouble(dt.Rows[i]["RequirementProgress"]);
                temp.DesignProgress = (float)Convert.ToDouble(dt.Rows[i]["DesignProgress"]);
                temp.DevelopmentProgress = (float)Convert.ToDouble(dt.Rows[i]["DevelopmentProgress"]);
                temp.TestProgress = (float)Convert.ToDouble(dt.Rows[i]["TestProgress"]);
                temp.UatProgress = (float)Convert.ToDouble(dt.Rows[i]["UatProgress"]);
                temp.OverallEpicCompletion = (float)Convert.ToDouble(dt.Rows[i]["OverallEpicCompletion"]);
                temp.PreviousMonthCumulativeActualEffort = (float)Convert.ToDouble(dt.Rows[i]["PreviousMonthCumulativeActualEffort"]);
                temp.ActualEffort = (float)Convert.ToDouble(dt.Rows[i]["ActualEffort"]);
                temp.UserName = Convert.ToString(dt.Rows[i]["UserName"]);
                temp.UserIp = Convert.ToString(dt.Rows[i]["UserIp"]);
                if (Convert.IsDBNull(dt.Rows[i]["TeamId"]) || Convert.IsDBNull(dt.Rows[i]["TeamName"]))
                {
                    temp.Team = new FeatureTeamModel() { TeamId = 0, TeamName = ""};
                }
                else
                {
                    temp.Team.TeamId = Convert.ToInt32(dt.Rows[i]["TeamId"]);
                    temp.Team.TeamName = Convert.ToString(dt.Rows[i]["TeamName"]);
                }
                FeatureList.Add(temp);
            }
            return FeatureList;
        }

        public IEnumerable<FeatureReport> GetFeatureReport(int featureId, int featureIsFSM, int epicId, int year, int month, int type)
        {
            int prevYear = month == 1 ? year - 1 : year;
            int prevMonth = month == 1 ? 12 : month - 1;
            List<FeatureReport> FeatureList = new List<FeatureReport>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_FeatureReport]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    if (featureId != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@FeatureId", featureId);
                    }
                    if (featureIsFSM != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@FeatureIsFSM", featureIsFSM);
                    }
                    if (epicId != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@EpicId", epicId);
                    }
                    if (year != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@Year", year);
                        sqlCommand.Parameters.AddWithValue("@PrevYear", prevYear);
                    }
                    if (month != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@Month", month);
                        sqlCommand.Parameters.AddWithValue("@PrevMonth", prevMonth);
                    }
                    if (type != 0)
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
                FeatureReport temp = new FeatureReport();
                temp.FeatureId = Convert.ToInt32(dt.Rows[i]["FeatureId"]);
                temp.FeatureName = Convert.ToString(dt.Rows[i]["FeatureName"]);
                temp.FeatureEstimation = Convert.ToInt32(dt.Rows[i]["FeatureEstimation"]);
                temp.FSM = Convert.ToString(dt.Rows[i]["FeatureIsFSMName"]);
                temp.EpicId = Convert.ToInt32(dt.Rows[i]["EpicId"]);
                temp.Year = Convert.ToInt32(dt.Rows[i]["Year"]);
                temp.Month = Convert.ToInt32(dt.Rows[i]["Month"]);
                temp.TypeValue = Convert.ToInt32(dt.Rows[i]["TypeValue"]);
                temp.TypeName = Convert.ToString(dt.Rows[i]["TypeName"]);
                temp.CurrentRequirementProgress = (float)Convert.ToDouble(dt.Rows[i]["RequirementProgress"]);
                temp.CurrentDesignProgress = (float)Convert.ToDouble(dt.Rows[i]["DesignProgress"]);
                temp.CurrentDevelopmentProgress = (float)Convert.ToDouble(dt.Rows[i]["DevelopmentProgress"]);
                temp.CurrentTestProgress = (float)Convert.ToDouble(dt.Rows[i]["TestProgress"]);
                temp.CurrentUatProgress = (float)Convert.ToDouble(dt.Rows[i]["UatProgress"]);
                temp.CurrentOverallEpicCompletion = (float)Convert.ToDouble(dt.Rows[i]["OverallEpicCompletion"]) / 100;
                temp.PrevMonthRequirementProgress = (float)Convert.ToDouble(dt.Rows[i]["PrevMonthRequirementProgress"]);
                temp.PrevMonthDesignProgress = (float)Convert.ToDouble(dt.Rows[i]["PrevMonthDesignProgress"]);
                temp.PrevMonthDevelopmentProgress = (float)Convert.ToDouble(dt.Rows[i]["PrevMonthDevelopmentProgress"]);
                temp.PrevMonthTestProgress = (float)Convert.ToDouble(dt.Rows[i]["PrevMonthTestProgress"]);
                temp.PrevMonthUatProgress = (float)Convert.ToDouble(dt.Rows[i]["PrevMonthUatProgress"]);
                temp.PrevMonthOverallEpicCompletion = (float)Convert.ToDouble(dt.Rows[i]["PrevMonthOverallEpicCompletion"])/100;
                if (Convert.IsDBNull(dt.Rows[i]["TeamId"]) || Convert.IsDBNull(dt.Rows[i]["TeamName"]))
                {
                    temp.Team = "";
                }
                else
                {
                    temp.Team = Convert.ToString(dt.Rows[i]["TeamName"]);
                }
                FeatureList.Add(temp);
            }
            return FeatureList;
        }

        public void DeleteFeature(Feature feature)
        {
            if (feature != null && feature.FeatureId != 0)
            {
                DataTable dt = new DataTable();
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    string procName = "[del_Feature]";
                    using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@FeatureId", feature.FeatureId);
                        sqlConnection.Open();
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                        {
                            sqlDataAdapter.Fill(dt);
                        }
                    }
                }
            }
        }

        public void InsertFeature(Feature feature)
        {
            Measurement measurement = null;
            List<Measurement> ms = (List<Measurement>)GetMeasurementAll(feature.EpicId, feature.Year, feature.Month, feature.TypeName, feature.Team.TeamId);
            if (ms != null && ms.Count > 0)
            {
                measurement = ms[0];
            }
            if (measurement != null)
            {
                DataTable dt = new DataTable();
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    string procName = "[ins_Feature]";
                    using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                    {
                        sqlCommand.CommandType = CommandType.StoredProcedure;
                        sqlCommand.Parameters.AddWithValue("@FeatureName", feature.FeatureName);
                        sqlCommand.Parameters.AddWithValue("@FeatureEstimation", feature.FeatureEstimation);
                        sqlCommand.Parameters.AddWithValue("@FeatureIsFSM", GetParameterValue("IsFirstSellableModule", measurement.IsFirstSellableModule));
                        sqlCommand.Parameters.AddWithValue("@EpicId", feature.EpicId);
                        sqlCommand.Parameters.AddWithValue("@Year", feature.Year);
                        sqlCommand.Parameters.AddWithValue("@Month", feature.Month);
                        sqlCommand.Parameters.AddWithValue("@Type", 2);
                        sqlCommand.Parameters.AddWithValue("@RequirementProgress", feature.RequirementProgress);
                        sqlCommand.Parameters.AddWithValue("@DesignProgress", feature.DesignProgress);
                        sqlCommand.Parameters.AddWithValue("@DevelopmentProgress", feature.DevelopmentProgress);
                        sqlCommand.Parameters.AddWithValue("@TestProgress", feature.TestProgress);
                        sqlCommand.Parameters.AddWithValue("@UatProgress", feature.UatProgress);
                        sqlCommand.Parameters.AddWithValue("@PreviousMonthCumulativeActualEffort", feature.PreviousMonthCumulativeActualEffort);
                        sqlCommand.Parameters.AddWithValue("@ActualEffort", feature.ActualEffort);
                        sqlCommand.Parameters.AddWithValue("@UserName", feature.UserName);
                        sqlCommand.Parameters.AddWithValue("@UserIp", feature.UserIp);
                        sqlCommand.Parameters.AddWithValue("@TeamId", measurement.Team.TeamId);
                        sqlConnection.Open();
                        using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                        {
                            sqlDataAdapter.Fill(dt);
                        }
                    }
                }
            }
        }

        public void UpdateFeature(Feature feature, string userName, string userIp)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[upd_Feature]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@FeatureId", feature.FeatureId);
                    sqlCommand.Parameters.AddWithValue("@FeatureName", feature.FeatureName);
                    sqlCommand.Parameters.AddWithValue("@FeatureEstimation", feature.FeatureEstimation);
                    sqlCommand.Parameters.AddWithValue("@FeatureIsFSM", feature.FSM.FSMValue);
                    sqlCommand.Parameters.AddWithValue("@EpicId", feature.EpicId);
                    sqlCommand.Parameters.AddWithValue("@Year", feature.Year);
                    sqlCommand.Parameters.AddWithValue("@Month", feature.Month);
                    sqlCommand.Parameters.AddWithValue("@Type", feature.TypeValue);
                    sqlCommand.Parameters.AddWithValue("@RequirementProgress", feature.RequirementProgress);
                    sqlCommand.Parameters.AddWithValue("@DesignProgress", feature.DesignProgress);
                    sqlCommand.Parameters.AddWithValue("@DevelopmentProgress", feature.DevelopmentProgress);
                    sqlCommand.Parameters.AddWithValue("@TestProgress", feature.TestProgress);
                    sqlCommand.Parameters.AddWithValue("@UatProgress", feature.UatProgress);
                    sqlCommand.Parameters.AddWithValue("@PreviousMonthCumulativeActualEffort", feature.PreviousMonthCumulativeActualEffort);
                    sqlCommand.Parameters.AddWithValue("@ActualEffort", feature.ActualEffort);
                    sqlCommand.Parameters.AddWithValue("@UserName", userName);
                    sqlCommand.Parameters.AddWithValue("@UserIp", userIp);
                    sqlCommand.Parameters.AddWithValue("@TeamId", feature.Team.TeamId);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
        }

        #endregion

        public IEnumerable<TeamProgressTrack> GetTeamProgressTrack(int year, int month, int isFSM)
        {
            List<Team> teams = new List<Team>();
            List<MeasurementDetailsViewModel> measurementList = new List<MeasurementDetailsViewModel>();
            int prevYear = month == 1 ? year - 1 : year;
            int prevMonth = month == 1 ? 12 : month - 1;
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
                    if (isFSM != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@IsFirstSellableModule", isFSM);
                    }
                    sqlCommand.Parameters.AddWithValue("@Location", GetParameterValue("ProjectLocation", "Turkey"));
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
                temp.Year = Convert.ToInt32(dt.Rows[i]["Year"]);
                temp.Month = Convert.ToInt32(dt.Rows[i]["Month"]);
                temp.Team.TeamName = Convert.ToString(dt.Rows[i]["TeamName"]);

                if (teams.Find(t => t.TeamName == temp.Team.TeamName) == null)
                {
                    teams.Add(new Team() { TeamName = temp.Team.TeamName});
                }

                if (Convert.IsDBNull(dt.Rows[i]["PreviousMonthRequirementProgress"]))
                {
                    temp.PrevMonthWeightedOverallProgress = 0;
                }
                else
                {
                    temp.PrevMonthWeightedOverallProgress = (float)Convert.ToDouble(dt.Rows[i]["PreviousMonthWeightedOverallProgress"]);
                }
                if (Convert.IsDBNull(dt.Rows[i]["ActualRequirementProgress"]))
                {
                    temp.ActualWeightedOverallProgress = 0;
                }
                else
                {
                    temp.ActualWeightedOverallProgress = (float)Convert.ToDouble(dt.Rows[i]["ActualWeightedOverallProgress"]);
                }
                if (Convert.IsDBNull(dt.Rows[i]["ActualRequirementProgress"]))
                {
                    temp.TargetWeightedOverallProgress = 0;
                }
                else
                {
                    temp.TargetWeightedOverallProgress = (float)Convert.ToDouble(dt.Rows[i]["TargetWeightedOverallProgress"]);
                }

                temp.EditMode = Convert.ToString(dt.Rows[i]["EditMode"]);

                measurementList.Add(temp);
            }


            List<TeamProgressTrack> track = new List<TeamProgressTrack>();
            for (int i = 0; i < teams.Count(); i++)
            {
                track.Add(new TeamProgressTrack() { Team = teams[i].TeamName});
            }

            for (int i = 0; i < measurementList.Count(); i++)
            {
                for (int j = 0; j < track.Count(); j++)
                {
                    if (track[j].Team == measurementList[i].Team.TeamName)
                    {
                        track[j].PreviousMonthProgress += measurementList[i].PrevMonthWeightedOverallProgress;
                        track[j].CurrentProgress += measurementList[i].ActualWeightedOverallProgress;
                        track[j].TargetProgress += measurementList[i].TargetWeightedOverallProgress;
                    }
                }
            }

            TeamProgressTrack shared = new TeamProgressTrack();
            TeamProgressTrack turkeyPool = new TeamProgressTrack();
            for (int i = 0; i < track.Count(); i++)
            {
                track[i].RealizationRate = ((track[i].CurrentProgress - track[i].PreviousMonthProgress) / (track[i].TargetProgress - track[i].PreviousMonthProgress)) * 100;
                if (track[i].Team == "Shared")
                {
                    shared = track[i];
                }
                if (track[i].Team == "Turkey Pool")
                {
                    turkeyPool = track[i];
                }
            }
            track.Remove(shared);
            track.Remove(turkeyPool);


            List<TeamProgressTrack> SortedList = track.OrderByDescending(team => team.RealizationRate).ToList();

            return SortedList;
        }

        public List<MeasurementDetailsViewModel> FillMeasurementDetails(int year, int month, string location, string isFSM, string team)
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
                    if (location != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@Location", GetParameterValue("ProjectLocation", location));
                    }
                    if (isFSM != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@IsFirstSellableModule", GetParameterValue("IsFirstSellableModule", isFSM));
                    }
                    if (team != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@Team", GetTeamIdByName(team));
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
                MeasurementDetailsViewModel temp = new MeasurementDetailsViewModel();
                temp.EpicId = Convert.ToInt32(dt.Rows[i]["EpicId"]);
                temp.EpicName = Convert.ToString(dt.Rows[i]["EpicName"]);
                temp.Module.ModuleName = Convert.ToString(dt.Rows[i]["ModuleName"]);
                temp.Year = Convert.ToInt32(dt.Rows[i]["Year"]);
                temp.Month = Convert.ToInt32(dt.Rows[i]["Month"]);
                temp.Location = Convert.ToString(dt.Rows[i]["ProjectLocation"]);
                temp.EpicWeight = (float)Convert.ToDouble(dt.Rows[i]["EpicWeight"]);
                temp.Estimation = (float)Convert.ToDouble(dt.Rows[i]["Estimation"]);
                temp.FSMPercentage = (float)Convert.ToDouble(dt.Rows[i]["FSMPercentage"]);
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
                temp.Deviation = (float)Convert.ToDouble(dt.Rows[i]["Deviation"]);

                MeasurementDetailsList.Add(temp);
            }


            return MeasurementDetailsList;
        }

        public List<Measurement> GenerateMeasurementForNextMonth(int year, int month, string location, string userName, string ipAddress)
        {
            int prevYear = (month == 1 ? year - 1 : year);
            int prevMonth = (month == 1 ? 12 : month - 1);
            List<Measurement> CurrentMeasurementList = (List<Measurement>)GetMeasurementAll(0, prevYear, prevMonth, null, 0);
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
                if (CurrentMeasurementList[i].Type.TypeValue == 1 && i < CurrentMeasurementList.Count() - 1)
                {
                    Measurement currentM = CurrentMeasurementList[i];
                    Measurement currentActual = CurrentMeasurementList[i+1];
                    if (currentM.RequirementProgress < currentActual.RequirementProgress)
                    {
                        temp.RequirementProgress = currentActual.RequirementProgress;
                    }
                    if (currentM.DesignProgress < currentActual.DesignProgress)
                    {
                        temp.DesignProgress = currentActual.DesignProgress;
                    }
                    if (currentM.DevelopmentProgress < currentActual.DevelopmentProgress)
                    {
                        temp.DevelopmentProgress = currentActual.DevelopmentProgress;
                    }
                    if (currentM.TestProgress < currentActual.TestProgress)
                    {
                        temp.TestProgress = currentActual.TestProgress;
                    }
                    if (currentM.UatProgress < currentActual.UatProgress)
                    {
                        temp.UatProgress = currentActual.UatProgress;
                    }
                }
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
            int prevYear = dates[0].Year;//datecontrol silinecek
            int prevMonth = dates[0].Month;
            List<Measurement> LastmonthMeasurement = (List<Measurement>)GetMeasurementAll(0, prevYear, prevMonth, null, 0);
            for (int i = 0; i < LastmonthMeasurement.Count(); i++)
            {
                DeleteMeasurement(LastmonthMeasurement[i].EpicId, prevYear, prevMonth, LastmonthMeasurement[i].Type.TypeValue, userName, ipAddress);
            }
            DeleteMeasurementLog(0, prevYear, prevMonth, 0);
        }
        
        public List<Measurement> SearchMeasurement(int year, int month, string location, string type, string teamName)
        {
            List<Measurement> list = new List<Measurement>();
            List<int> IdListByLocation = GetEpicBaseLineIdByLocation(location);
            List<int> IdListByTeam = GetEpicBaseLineIdByTeam(teamName);
            List<Measurement> measurements = (List<Measurement>)GetMeasurementAll(0, year, month, type, 0);
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
                if (isFirstSellableModule == "Phase-4")
                {
                    sum = sum + (float)Convert.ToDouble(dt.Rows[i]["EpicWeight"])*((float)Convert.ToDouble(dt.Rows[i]["FSMPercentage"]) / 100);
                }
                else
                {
                    sum = sum + (float)Convert.ToDouble(dt.Rows[i]["EpicWeight"]);
                }
            }
            return sum;
        }

        public List<Module> GetModuleProgress(int year, int month, int isFSM, string location)
        {
            List<Module> modules = (List<Module>)GetModuleAll();
            List<Module> moduleAggregates = GetModuleAggregates(year, month, isFSM, location);
            List<Module> epics = new List<Module>(); 
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_ModuleProgress]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@Year", year);
                    sqlCommand.Parameters.AddWithValue("@Month", month);
                    if (location != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@ProjectLocation", GetParameterValue("ProjectLocation", location));
                    }
                    if (isFSM != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@FirstSellableModule", isFSM);
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
                temp.ModuleName = Convert.ToString(dt.Rows[i]["ModuleName"]);
                temp.RequirementProgress = (float)Convert.ToDouble(dt.Rows[i]["RequirementProgress"]);
                temp.DesignProgress = (float)Convert.ToDouble(dt.Rows[i]["DesignProgress"]);
                temp.DevelopmentProgress = (float)Convert.ToDouble(dt.Rows[i]["DevelopmentProgress"]);
                temp.TestProgress = (float)Convert.ToDouble(dt.Rows[i]["TestProgress"]);
                temp.UatProgress = (float)Convert.ToDouble(dt.Rows[i]["UatProgress"]);
                temp.Progress = (float)Convert.ToDouble(dt.Rows[i]["EpicProgress"]);
                temp.Weight = (float)Convert.ToDouble(dt.Rows[i]["EpicWeight"]);
                temp.Variance = (float)Convert.ToDouble(dt.Rows[i]["Variance"]);
                temp.ActualEffort = (float)Convert.ToDouble(dt.Rows[i]["ActualEffort"]);
                temp.WeightedOverallProgress = (float)Convert.ToDouble(dt.Rows[i]["OverallCompilation"]);
                temp.FSMPercentage = (float)Convert.ToDouble(dt.Rows[i]["FSMPercentage"]);
                epics.Add(temp); 
            }
            
            for (int i = 0; i < epics.Count(); i++)
            {
                for (int j = 0; j < modules.Count(); j++)
                {
                    if (modules[j].ModuleId == epics[i].ModuleId)
                    {
                        modules[j].WeightedOverallProgress = modules[j].WeightedOverallProgress + epics[i].WeightedOverallProgress;
                        if (isFSM == 4)
                        {
                            modules[j].Weight = modules[j].Weight + epics[i].Weight * (epics[i].FSMPercentage / 100);
                        }
                        else
                        {
                            modules[j].Weight = modules[j].Weight + epics[i].Weight;
                        }
                        modules[j].Variance = modules[j].Variance + epics[i].Variance;
                        modules[j].ActualEffort = modules[j].ActualEffort + epics[i].ActualEffort;
                    }
                }
            }
            for (int i = 0; i < epics.Count(); i++)
            {
                for (int j = 0; j < modules.Count(); j++)
                {
                    if (modules[j].ModuleId == epics[i].ModuleId)
                    {
                        if (isFSM == 4)
                        {
                            float progress = epics[i].Progress >= epics[i].FSMPercentage ? 100 : epics[i].Progress / epics[i].FSMPercentage * 100;
                            float reqProgress = epics[i].RequirementProgress >= epics[i].FSMPercentage ? 100 : epics[i].RequirementProgress / epics[i].FSMPercentage * 100;
                            float desProgress = epics[i].DesignProgress >= epics[i].FSMPercentage ? 100 : epics[i].DesignProgress / epics[i].FSMPercentage * 100;
                            float devProgress = epics[i].DevelopmentProgress >= epics[i].FSMPercentage ? 100 : epics[i].DevelopmentProgress / epics[i].FSMPercentage * 100;
                            float testProgress = epics[i].TestProgress >= epics[i].FSMPercentage ? 100 : epics[i].TestProgress / epics[i].FSMPercentage * 100;
                            float uatProgress = epics[i].UatProgress >= epics[i].FSMPercentage ? 100 : epics[i].UatProgress / epics[i].FSMPercentage * 100;
                            modules[j].Progress = modules[j].Progress + progress * (epics[i].Weight * epics[i].FSMPercentage / 100) / modules[j].Weight;
                            modules[j].RequirementProgress = modules[j].RequirementProgress + reqProgress * (epics[i].Weight * epics[i].FSMPercentage / 100) / modules[j].Weight;
                            modules[j].DesignProgress = modules[j].DesignProgress + desProgress * (epics[i].Weight * epics[i].FSMPercentage / 100) / modules[j].Weight;
                            modules[j].DevelopmentProgress = modules[j].DevelopmentProgress + devProgress * (epics[i].Weight * epics[i].FSMPercentage / 100) / modules[j].Weight;
                            modules[j].TestProgress = modules[j].TestProgress + testProgress * (epics[i].Weight * epics[i].FSMPercentage / 100) / modules[j].Weight;
                            modules[j].UatProgress = modules[j].UatProgress + uatProgress * (epics[i].Weight * epics[i].FSMPercentage / 100) / modules[j].Weight;
                        }
                        else
                        {
                            modules[j].Progress = modules[j].Progress + epics[i].Progress * (epics[i].Weight / modules[j].Weight);
                            modules[j].RequirementProgress = modules[j].RequirementProgress + epics[i].RequirementProgress * (epics[i].Weight / modules[j].Weight);
                            modules[j].DesignProgress = modules[j].DesignProgress + epics[i].DesignProgress * (epics[i].Weight / modules[j].Weight);
                            modules[j].DevelopmentProgress = modules[j].DevelopmentProgress + epics[i].DevelopmentProgress * (epics[i].Weight / modules[j].Weight);
                            modules[j].TestProgress = modules[j].TestProgress + epics[i].TestProgress * (epics[i].Weight / modules[j].Weight);
                            modules[j].UatProgress = modules[j].UatProgress + epics[i].UatProgress * (epics[i].Weight / modules[j].Weight);
                        }
                    }
                }
            }
            
            
            List<Module> SortedList = modules.OrderByDescending(mo => mo.Progress).ToList();
            for (int i = 0; i < SortedList.Count(); i++)
            {
                for (int j = 0; j < moduleAggregates.Count(); j++)
                {
                    if (SortedList[i].ModuleId == moduleAggregates[j].ModuleId)
                    {
                        SortedList[i].EpicCount = moduleAggregates[j].EpicCount;
                        SortedList[i].EpicString = moduleAggregates[j].EpicCount.ToString();
                        SortedList[i].TotalEstimation = moduleAggregates[j].TotalEstimation;
                    }
                }
            }
            List<Module> ms = new List<Module>();
            for (int i = 0; i < SortedList.Count(); i++)
            {
                if (SortedList[i].EpicCount == 0)
                {
                    ms.Add(SortedList[i]);
                }
            }
            for (int i = 0; i < ms.Count(); i++)
            {
                SortedList.Remove(ms[i]);
            }
            if (isFSM == 4)
            {
                List<Module> list = GetModuleAggregates(year, month, 0, location);
                int fsmCount = 0;
                int totalCount = 0;
                for (int i = 0; i < list.Count(); i++)
                {
                    for (int j = 0; j < SortedList.Count(); j++)
                    {
                        if (list[i].ModuleId == SortedList[j].ModuleId)
                        {
                            fsmCount += SortedList[j].EpicCount;
                            totalCount += list[i].EpicCount;
                            SortedList[j].EpicString = SortedList[j].EpicCount + " / " + list[i].EpicCount;
                        }
                    }
                }
                SortedList[0].EpicCountFooter = fsmCount + " / " + totalCount;
            }
            
            return SortedList;
        }

        public List<Module> GetModuleAggregates(int year, int month, int isFSM, string location)
        {
            List<Module> moduleList = new List<Module>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_ModuleEpicCount]";
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
                    if (isFSM != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@FirstSellableModule", isFSM);
                    }
                    if (location != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@ProjectLocation", GetParameterValue("ProjectLocation", location));
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
                moduleList.Add(temp);
            }

            dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_ModuleEstimation]";
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
                    if (isFSM == 4 || isFSM == 5 || isFSM == 6)
                    {
                        sqlCommand.Parameters.AddWithValue("@FirstSellableModule", isFSM);
                    }
                    if (location != null)
                    {
                        sqlCommand.Parameters.AddWithValue("@ProjectLocation", GetParameterValue("ProjectLocation", location));
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
                temp.TotalEstimation = (float)Convert.ToDouble(dt.Rows[i]["Estimation"]);
                temp.FSMPercentage = (float)Convert.ToDouble(dt.Rows[i]["FSMPercentage"]);
                for (int j = 0; j < moduleList.Count(); j++)
                {
                    if (temp.ModuleId == moduleList[j].ModuleId)
                    {
                        if (isFSM == 4)
                        {
                            moduleList[j].TotalEstimation += temp.TotalEstimation * temp.FSMPercentage;
                        }
                        //else if (isFSM == 2)
                        //{
                        //    moduleList[j].TotalEstimation += temp.TotalEstimation * (1 - temp.FSMPercentage);
                        //}
                        else
                        {
                            moduleList[j].TotalEstimation += temp.TotalEstimation;
                        }
                    }
                }
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
        public IEnumerable<HighLevelProgress> GetHighLevelProgressNew(string location, string isFirstSellableModule, Date date)
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
                string procName = "[sel_HighLevelProgressNew]";
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
        public string GetBackup(string command)
        {
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = command,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (string.IsNullOrEmpty(error)) { return output; }
            else { return error; }
        }

        public void MeasurementLogOldToNew(int epicId, int year, int month, int type)
        {
            List<EpicBaseLine> epics = (List<EpicBaseLine>)GetEpicBaseLineAll(0);

            for (int i = 0; i < epics.Count(); i++)
            {
                List<MeasurementLogModel> logs = (List<MeasurementLogModel>)GetLogMeasurement(epics[i].EPICId, 2020, 1, "Target", null);
                for (int j = 0; j < logs.Count() - 1; j++)
                {
                    MeasurementLog temp = new MeasurementLog();
                    temp.EpicId = logs[i].EpicId;
                    temp.Year = logs[i].EpicId;
                    temp.Month = logs[i].Month;
                    temp.Type = logs[i].Type;
                    temp.OldValues.RequirementProgress = logs[i].RequirementProgress;
                    temp.OldValues.DesignProgress = logs[i].DesignProgress;
                    temp.OldValues.DevelopmentProgress = logs[i].DevelopmentProgress;
                    temp.OldValues.TestProgress = logs[i].TestProgress;
                    temp.OldValues.UatProgress = logs[i].UatProgress;
                    temp.OldValues.PreviousMonthCumulativeActualEffort = logs[i].PreviousMonthCumulativeActualEffort;
                    temp.OldValues.ActualEffort = logs[i].ActualEffort;
                    temp.NewValues.RequirementProgress = logs[i+1].RequirementProgress;
                    temp.NewValues.DesignProgress = logs[i + 1].DesignProgress;
                    temp.NewValues.DevelopmentProgress = logs[i + 1].DevelopmentProgress;
                    temp.NewValues.TestProgress = logs[i + 1].TestProgress;
                    temp.NewValues.UatProgress = logs[i + 1].UatProgress;
                    temp.NewValues.PreviousMonthCumulativeActualEffort = logs[i + 1].PreviousMonthCumulativeActualEffort;
                    temp.NewValues.ActualEffort = logs[i + 1].ActualEffort;
                    temp.UserName = logs[i].UserName;
                    temp.Time = logs[i].Time;
                }
            }
        }
        public List<MeasurementLog> GetLogMeasurement2(int epicId, int year, int month, string type, string user)
        {
            List<EpicBaseLine> epics = (List<EpicBaseLine>)GetEpicBaseLineAll(0);
            List<MeasurementLogModel> logList = new List<MeasurementLogModel>();
            List<MeasurementLog> longLogList = new List<MeasurementLog>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Measurement_Logs]";
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
                    if (type == null)
                    {
                        sqlCommand.Parameters.AddWithValue("@Type", null);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@Type", GetParameterValue("Type", type));
                    }
                    if (user == null)
                    {
                        sqlCommand.Parameters.AddWithValue("@UserName", null);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@UserName", user);
                    }
                    if (epicId != 0)
                    {
                        sqlCommand.Parameters.AddWithValue("@EpicId", epicId);
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
                if (Convert.ToString(dt.Rows[i]["UserName"]) != "Egypt Test Admin" && Convert.ToString(dt.Rows[i]["UserName"]) != "Turkey Test Admin")
                {
                    MeasurementLogModel temp = new MeasurementLogModel();
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
                    if (!Convert.IsDBNull(dt.Rows[i]["UserName"]))
                    {
                        temp.UserName = Convert.ToString(dt.Rows[i]["UserName"]);
                    }
                    temp.Time = Convert.ToDateTime(dt.Rows[i]["Time"]).ToLocalTime().ToString("dd/MM/yyyy HH:mm");
                    //if (!Convert.IsDBNull(dt.Rows[i]["UserIp"]))
                    //{
                    //    temp.UserIp = Convert.ToString(dt.Rows[i]["UserIp"]);
                    //}
                    logList.Add(temp);
                }
            }
            for (int j = 0; j < epics.Count(); j++)
            {
                List<MeasurementLogModel> modelList = logList.FindAll(l => l.EpicId == epics[j].EPICId && l.Type == "Target");
                for (int k = 0; k < modelList.Count()-1; k++)
                {
                    MeasurementLog temp = new MeasurementLog();
                    temp.EpicId = modelList[k].EpicId;
                    temp.EpicName = modelList[k].EpicName;
                    temp.Year = modelList[k].Year;
                    temp.Month = modelList[k].Month;
                    temp.Module = modelList[k].Module;
                    temp.Team = modelList[k].Team;
                    temp.Type = modelList[k].Type;
                    temp.NewValues.RequirementProgress = modelList[k].RequirementProgress;
                    temp.NewValues.DesignProgress = modelList[k].DesignProgress;
                    temp.NewValues.DevelopmentProgress = modelList[k].DevelopmentProgress;
                    temp.NewValues.TestProgress = modelList[k].TestProgress;
                    temp.NewValues.UatProgress = modelList[k].UatProgress;
                    temp.NewValues.PreviousMonthCumulativeActualEffort = modelList[k].PreviousMonthCumulativeActualEffort;
                    temp.NewValues.ActualEffort = modelList[k].ActualEffort;
                    temp.OldValues.RequirementProgress = modelList[k + 1].RequirementProgress;
                    temp.OldValues.DesignProgress = modelList[k + 1].DesignProgress;
                    temp.OldValues.DevelopmentProgress = modelList[k + 1].DevelopmentProgress;
                    temp.OldValues.TestProgress = modelList[k + 1].TestProgress;
                    temp.OldValues.UatProgress = modelList[k + 1].UatProgress;
                    temp.OldValues.PreviousMonthCumulativeActualEffort = modelList[k + 1].PreviousMonthCumulativeActualEffort;
                    temp.OldValues.ActualEffort = modelList[k + 1].ActualEffort;
                    temp.UserName = modelList[k].UserName;
                    temp.Time = modelList[k].Time;
                    longLogList.Add(temp);
                }
                if (modelList.Count() > 0)
                {
                    List<Date> dates = GetDates();
                    Date temp = null;
                    Measurement m = new Measurement();
                    for (int l = 0; l < dates.Count(); l++)
                    {
                        if ((dates[l].Year == year && dates[l].Month == month) && (dates[l+1] != null))
                        {
                            temp = dates[l + 1];
                        }
                    }
                    if (temp != null)
                    {
                        m = ((List<Measurement>)GetMeasurementAll(modelList[0].EpicId, temp.Year, temp.Month, "Target", 0))[0];
                    }
                    MeasurementLog t = new MeasurementLog();
                    t.EpicId = modelList[0].EpicId;
                    t.EpicName = modelList[0].EpicName;
                    t.Year = modelList[0].Year;
                    t.Month = modelList[0].Month;
                    t.Module = modelList[0].Module;
                    t.Team = modelList[0].Team;
                    t.Type = modelList[0].Type;
                    int idx = modelList.Count() - 1;
                    t.NewValues.RequirementProgress = modelList[idx].RequirementProgress;
                    t.NewValues.DesignProgress = modelList[idx].DesignProgress;
                    t.NewValues.DevelopmentProgress = modelList[idx].DevelopmentProgress;
                    t.NewValues.TestProgress = modelList[idx].TestProgress;
                    t.NewValues.UatProgress = modelList[idx].UatProgress;
                    t.NewValues.PreviousMonthCumulativeActualEffort = modelList[idx].PreviousMonthCumulativeActualEffort;
                    t.NewValues.ActualEffort = modelList[idx].ActualEffort;
                    t.OldValues.RequirementProgress = m.RequirementProgress;
                    t.OldValues.DesignProgress = m.DesignProgress;
                    t.OldValues.DevelopmentProgress = m.DevelopmentProgress;
                    t.OldValues.TestProgress = m.TestProgress;
                    t.OldValues.UatProgress = m.UatProgress;
                    t.OldValues.PreviousMonthCumulativeActualEffort = m.PreviousMonthCumulativeActualEffort;
                    t.OldValues.ActualEffort = m.ActualEffort;
                    t.UserName = modelList[idx].UserName;
                    t.Time = modelList[idx].Time;
                    longLogList.Add(t);
                }


                modelList = logList.FindAll(l => l.EpicId == epics[j].EPICId && l.Type == "Actual");
                for (int k = 0; k < modelList.Count()-1; k++)
                {
                    MeasurementLog temp = new MeasurementLog();
                    temp.EpicId = modelList[k].EpicId;
                    temp.EpicName = modelList[k].EpicName;
                    temp.Year = modelList[k].Year;
                    temp.Month = modelList[k].Month;
                    temp.Type = modelList[k].Type;
                    temp.Module = modelList[k].Module;
                    temp.Team = modelList[k].Team;
                    temp.NewValues.RequirementProgress = modelList[k].RequirementProgress;
                    temp.NewValues.DesignProgress = modelList[k].DesignProgress;
                    temp.NewValues.DevelopmentProgress = modelList[k].DevelopmentProgress;
                    temp.NewValues.TestProgress = modelList[k].TestProgress;
                    temp.NewValues.UatProgress = modelList[k].UatProgress;
                    temp.NewValues.PreviousMonthCumulativeActualEffort = modelList[k].PreviousMonthCumulativeActualEffort;
                    temp.NewValues.ActualEffort = modelList[k].ActualEffort;
                    temp.OldValues.RequirementProgress = modelList[k + 1].RequirementProgress;
                    temp.OldValues.DesignProgress = modelList[k + 1].DesignProgress;
                    temp.OldValues.DevelopmentProgress = modelList[k + 1].DevelopmentProgress;
                    temp.OldValues.TestProgress = modelList[k + 1].TestProgress;
                    temp.OldValues.UatProgress = modelList[k + 1].UatProgress;
                    temp.OldValues.PreviousMonthCumulativeActualEffort = modelList[k + 1].PreviousMonthCumulativeActualEffort;
                    temp.OldValues.ActualEffort = modelList[k + 1].ActualEffort;
                    temp.UserName = modelList[k].UserName;
                    temp.Time = modelList[k].Time;
                    longLogList.Add(temp);
                }

                if (modelList.Count() > 0)
                {
                    List<Date> dates = GetDates();
                    Date temp = null;
                    Measurement m = new Measurement();
                    for (int l = 0; l < dates.Count(); l++)
                    {
                        if ((dates[l].Year == year && dates[l].Month == month) && (dates[l + 1] != null))
                        {
                            temp = dates[l + 1];
                        }
                    }
                    if (temp != null)
                    {
                        m = ((List<Measurement>)GetMeasurementAll(modelList[0].EpicId, temp.Year, temp.Month, "Actual", 0))[0];
                    }
                    MeasurementLog t = new MeasurementLog();
                    t.EpicId = modelList[0].EpicId;
                    t.EpicName = modelList[0].EpicName;
                    t.Year = modelList[0].Year;
                    t.Month = modelList[0].Month;
                    t.Module = modelList[0].Module;
                    t.Team = modelList[0].Team;
                    t.Type = modelList[0].Type;
                    int idx = modelList.Count() - 1;
                    t.NewValues.RequirementProgress = modelList[idx].RequirementProgress;
                    t.NewValues.DesignProgress = modelList[idx].DesignProgress;
                    t.NewValues.DevelopmentProgress = modelList[idx].DevelopmentProgress;
                    t.NewValues.TestProgress = modelList[idx].TestProgress;
                    t.NewValues.UatProgress = modelList[idx].UatProgress;
                    t.NewValues.PreviousMonthCumulativeActualEffort = modelList[idx].PreviousMonthCumulativeActualEffort;
                    t.NewValues.ActualEffort = modelList[idx].ActualEffort;
                    t.OldValues.RequirementProgress = m.RequirementProgress;
                    t.OldValues.DesignProgress = m.DesignProgress;
                    t.OldValues.DevelopmentProgress = m.DevelopmentProgress;
                    t.OldValues.TestProgress = m.TestProgress;
                    t.OldValues.UatProgress = m.UatProgress;
                    t.OldValues.PreviousMonthCumulativeActualEffort = m.PreviousMonthCumulativeActualEffort;
                    t.OldValues.ActualEffort = m.ActualEffort;
                    t.UserName = modelList[idx].UserName;
                    t.Time = modelList[idx].Time;
                    longLogList.Add(t);
                }
            }

            return longLogList;
        }

        public float GetTotalEstimation()
        {
            List<ProgressProducingVarianceAnalysis> varList = new List<ProgressProducingVarianceAnalysis>();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_TotalEpicEstimation]";
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
            float estimation = (float)Convert.ToDouble(dt.Rows[0]["TotalEstimation"]);
            return estimation;
        }

    }
}
