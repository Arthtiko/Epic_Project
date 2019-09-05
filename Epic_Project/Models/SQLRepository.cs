using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public class SQLRepository : IRepository
    {
        private string connectionString = "server=(localdb)\\MSSQLLocalDB;database=EPICDB;Trusted_Connection=True"; //düzelt
        private List<Team> TeamList = new List<Team>();
        private List<Module> ModuleList = new List<Module>();
        private List<EpicBaseLine> EpicBaseLineList = new List<EpicBaseLine>();
        private List<Measurement> MeasurementList = new List<Measurement>();
        private List<Employee> EmployeeList = new List<Employee>();
        private List<Parameter> ParameterList = new List<Parameter>();
        private List<Date> DateList = new List<Date>();


        #region Insert

        public EpicBaseLine InsertEpicBaseLine(EpicBaseLine epicBaseLine)
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
                    if (epicBaseLine.TeamName.TeamName == null)
                    {
                        epicBaseLine.TeamName.TeamName = "";
                        sqlCommand.Parameters.AddWithValue("@TeamId", 0);
                    }
                    else
                    {
                        sqlCommand.Parameters.AddWithValue("@TeamId", GetTeamIdByName(epicBaseLine.TeamName.TeamName));
                    }
                    sqlCommand.Parameters.AddWithValue("@IsMurabaha", GetParameterValue("IsMurabaha", epicBaseLine.IsMurabaha.MurabahaName));
                    sqlCommand.Parameters.AddWithValue("@IsFirstSellableModule", GetParameterValue("IsFirstSellableModule", epicBaseLine.IsFirstSellableModule.FirstSellableModuleName));
                    sqlCommand.Parameters.AddWithValue("@DistributedUnmappedEffort", epicBaseLine.DistributedUnmappedEffort);
                    sqlCommand.Parameters.AddWithValue("@TotalActualEffort", epicBaseLine.TotalActualEffort);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return epicBaseLine;
        }

        public Measurement InsertMeasurement(Measurement measurement)
        {
            if (measurement.EpicId != GetEpicBaseLineById(measurement.EpicId).EPICId)
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
                    //sqlCommand.Parameters.AddWithValue("@ModuleId", module.ModuleId); //identity
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
            List<Team> teams = (List<Team>)GetTeamAll();
            Team te = teams.Find(t => t.TeamName == team.TeamName);
            if (te != null)
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
                    //sqlCommand.Parameters.AddWithValue("@TeamId", team.TeamId);   //identity
                    sqlCommand.Parameters.AddWithValue("@TeamName", team.TeamName);
                    sqlCommand.Parameters.AddWithValue("@TeamLeaderId", GetEmployeeIdByName(team.TeamLeader.EmployeeName));
                    sqlCommand.Parameters.AddWithValue("@ProjectManagerId", GetEmployeeIdByName(team.ProjectManager.EmployeeName));
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
                    //sqlCommand.Parameters.AddWithValue("@EmployeeId", employee.EmployeeId);   //identity
                    sqlCommand.Parameters.AddWithValue("@EmployeeName", employee.EmployeeName);
                    sqlCommand.Parameters.AddWithValue("@EmployeeType", GetParameterValue("EmployeeType", employee.EmployeeType.TypeName));
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

        public void DeleteEpicBaseLine(int id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[del_EpicBaseLine]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@EPICId", id);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
        }

        public void DeleteMeasurement(int epicId, int year, int month, int type)
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

        public IEnumerable<EpicBaseLine> GetEpicBaseLineAll()
        {
            float TotalEstimation = GetTotalEpicEstimation();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_EpicBaseLine]";
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
                EpicBaseLine temp = new EpicBaseLine();
                temp.EPICId = Convert.ToInt32(dt.Rows[i]["EPICId"]);
                temp.EPICName = Convert.ToString(dt.Rows[i]["EPICName"]);
                temp.ModuleName.ModuleName = Convert.ToString(dt.Rows[i]["ModuleName"]);
                temp.ModuleName.ModuleId = GetModuleIdByName(temp.ModuleName.ModuleName);
                temp.EpicType.TypeName = Convert.ToString(dt.Rows[i]["EpicType"]);
                temp.EpicType.TypeValue = GetParameterValue("EpicType", temp.EpicType.TypeName);
                temp.ProjectLocation.LocationName = Convert.ToString(dt.Rows[i]["ProjectLocation"]);
                temp.ProjectLocation.LocationValue = GetParameterValue("ProjectLocation", temp.ProjectLocation.LocationName);
                temp.Estimation = (float)Convert.ToDouble(dt.Rows[i]["Estimation"]);
                temp.EpicWeight = temp.Estimation / TotalEstimation;
                temp.TeamName.TeamName = Convert.ToString(dt.Rows[i]["TeamName"]);
                if (temp.TeamName.TeamName != "")
                {
                    int tid = GetTeamIdByName(temp.TeamName.TeamName);
                    temp.TeamName = GetTeamById(tid);
                }
                else
                {
                    temp.TeamName = new Team() { TeamId = 0, TeamName = "", TeamLeader = new EmployeeViewModel(), ProjectManager = new EmployeeViewModel() };
                }
                temp.IsMurabaha.MurabahaName = Convert.ToString(dt.Rows[i]["IsMurabaha"]);
                temp.IsMurabaha.MurabahaValue = GetParameterValue("IsMurabaha", temp.IsMurabaha.MurabahaName);
                temp.IsFirstSellableModule.FirstSellableModuleName = Convert.ToString(dt.Rows[i]["IsFirstSellableModule"]);
                temp.IsFirstSellableModule.FirstSellableModuleValue = GetParameterValue("IsFirstSellableModule", temp.IsFirstSellableModule.FirstSellableModuleName);
                temp.DistributedUnmappedEffort = (float)Convert.ToDouble(dt.Rows[i]["DistributedUnmappedEffort"]);
                temp.TotalActualEffort = (float)Convert.ToDouble(dt.Rows[i]["TotalActualEffort"]);
                temp.ActualEffort = temp.TotalActualEffort - temp.DistributedUnmappedEffort;
                EpicBaseLineList.Add(temp);
            }
            return EpicBaseLineList;
        }

        public EpicBaseLine GetEpicBaseLineById(int id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_EpicBaseLine]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@EPICId", id);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            EpicBaseLine epicBaseLine = new EpicBaseLine();
            epicBaseLine.EPICId = Convert.ToInt32(dt.Rows[0]["EPICId"]);
            epicBaseLine.EPICName = Convert.ToString(dt.Rows[0]["EPICName"]);
            epicBaseLine.ModuleName.ModuleName = Convert.ToString(dt.Rows[0]["ModuleName"]);
            epicBaseLine.ModuleName.ModuleId = GetModuleIdByName(epicBaseLine.ModuleName.ModuleName);
            epicBaseLine.EpicType.TypeName = Convert.ToString(dt.Rows[0]["EpicType"]);
            epicBaseLine.EpicType.TypeValue = GetParameterValue("EpicType", epicBaseLine.EpicType.TypeName);
            epicBaseLine.ProjectLocation.LocationName = Convert.ToString(dt.Rows[0]["ProjectLocation"]);
            epicBaseLine.ProjectLocation.LocationValue = GetParameterValue("ProjectLocation", epicBaseLine.ProjectLocation.LocationName);
            epicBaseLine.Estimation = Convert.ToInt32(dt.Rows[0]["Estimation"]);
            epicBaseLine.TeamName.TeamName = Convert.ToString(dt.Rows[0]["TeamName"]);
            if (epicBaseLine.TeamName.TeamName != "")
            {
                int tid = GetTeamIdByName(epicBaseLine.TeamName.TeamName);
                epicBaseLine.TeamName = GetTeamById(tid);
            }
            else
            {
                epicBaseLine.TeamName = new Team() { TeamId = 0, TeamName = "", TeamLeader = new EmployeeViewModel(), ProjectManager = new EmployeeViewModel() };
            }
            epicBaseLine.IsMurabaha.MurabahaName = Convert.ToString(dt.Rows[0]["IsMurabaha"]);
            epicBaseLine.IsMurabaha.MurabahaValue = GetParameterValue("IsMurabaha", epicBaseLine.IsMurabaha.MurabahaName);
            epicBaseLine.IsFirstSellableModule.FirstSellableModuleName = Convert.ToString(dt.Rows[0]["IsFirstSellableModule"]);
            epicBaseLine.IsFirstSellableModule.FirstSellableModuleValue = GetParameterValue("IsFirstSellableModule", epicBaseLine.IsFirstSellableModule.FirstSellableModuleName); ;
            epicBaseLine.DistributedUnmappedEffort = (float)Convert.ToDouble(dt.Rows[0]["DistributedUnmappedEffort"]);
            epicBaseLine.TotalActualEffort = (float)Convert.ToDouble(dt.Rows[0]["TotalActualEffort"]);
            return epicBaseLine;
        }

        public IEnumerable<Measurement> GetMeasurementAll()
        {
            float totalEstimation = GetTotalEpicEstimation();
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Measurement]";
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
                Measurement temp = new Measurement();
                temp.EpicId = Convert.ToInt32(dt.Rows[i]["EpicId"]);
                temp.EpicName = Convert.ToString(dt.Rows[i]["EpicName"]);
                temp.Module.ModuleId = Convert.ToInt32(dt.Rows[i]["ModuleId"]);
                temp.Module.ModuleName = GetModuleById(temp.Module.ModuleId).ModuleName;
                temp.Year = Convert.ToInt32(dt.Rows[i]["Year"]);
                temp.Month = Convert.ToInt32(dt.Rows[i]["Month"]);
                temp.Type.TypeName = Convert.ToString(dt.Rows[i]["Type"]);
                temp.Type.TypeValue = GetParameterValue("Type", temp.Type.TypeName);
                float estimation = GetEstimationById(temp.EpicId);
                temp.EpicWeight = estimation / totalEstimation;
                if (Convert.IsDBNull(dt.Rows[i]["TeamId"]))
                {
                    temp.Team.TeamId = 0;
                    temp.Team.TeamName = "";
                    temp.Team.TeamLeader = new EmployeeViewModel();
                    temp.Team.ProjectManager = new EmployeeViewModel();
                }
                else
                {
                    temp.Team = GetTeamById(Convert.ToInt32(dt.Rows[i]["TeamId"]));
                }
                temp.RequirementProgress = (float)Convert.ToDouble(dt.Rows[i]["RequirementProgress"]) * 100;
                temp.DesignProgress = (float)Convert.ToDouble(dt.Rows[i]["DesignProgress"]) * 100;
                temp.DevelopmentProgress = (float)Convert.ToDouble(dt.Rows[i]["DevelopmentProgress"]) * 100;
                temp.TestProgress = (float)Convert.ToDouble(dt.Rows[i]["TestProgress"]) * 100;
                temp.UatProgress = (float)Convert.ToDouble(dt.Rows[i]["UatProgress"]) * 100;
                temp.PreviousMonthCumulativeActualEffort = (float)Convert.ToDouble(dt.Rows[i]["PreviousMonthCumulativeActualEffort"]);
                temp.ActualEffort = (float)Convert.ToDouble(dt.Rows[i]["ActualEffort"]);
                MeasurementList.Add(temp);
            }
            return MeasurementList;
        }

        public IEnumerable<Measurement> GetMeasurementAll(int epicId, int year, int month, string type)
        {
            float totalEstimation = GetTotalEpicEstimation();
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
                temp.Module = GetModuleById(temp.Module.ModuleId);
                temp.Year = Convert.ToInt32(dt.Rows[i]["Year"]);
                temp.Month = Convert.ToInt32(dt.Rows[i]["Month"]);
                temp.Type.TypeName = Convert.ToString(dt.Rows[i]["Type"]);
                temp.Type.TypeValue = GetParameterValue("Type", temp.Type.TypeName);
                float estimation = GetEstimationById(temp.EpicId);
                temp.EpicWeight = estimation / totalEstimation;
                if (Convert.IsDBNull(dt.Rows[i]["TeamId"]))
                {
                    temp.Team.TeamId = 0;
                    temp.Team.TeamName = "";
                    temp.Team.TeamLeader = new EmployeeViewModel();
                    temp.Team.ProjectManager = new EmployeeViewModel();
                }
                else
                {
                    temp.Team = GetTeamById(Convert.ToInt32(dt.Rows[i]["TeamId"]));
                }
                temp.RequirementProgress = (float)Convert.ToDouble(dt.Rows[i]["RequirementProgress"]) * 100;
                temp.DesignProgress = (float)Convert.ToDouble(dt.Rows[i]["DesignProgress"]) * 100;
                temp.DevelopmentProgress = (float)Convert.ToDouble(dt.Rows[i]["DevelopmentProgress"]) * 100;
                temp.TestProgress = (float)Convert.ToDouble(dt.Rows[i]["TestProgress"]) * 100;
                temp.UatProgress = (float)Convert.ToDouble(dt.Rows[i]["UatProgress"]) * 100;
                temp.PreviousMonthCumulativeActualEffort = (float)Convert.ToDouble(dt.Rows[i]["PreviousMonthCumulativeActualEffort"]);
                temp.ActualEffort = (float)Convert.ToDouble(dt.Rows[i]["ActualEffort"]);
                MeasurementList.Add(temp);
            }
            return MeasurementList;
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
                ModuleList.Add(temp);
            }
            return ModuleList;
        }

        public Module GetModuleById(int id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Module]";
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
            Module module = new Module();
            module.ModuleId = Convert.ToInt32(dt.Rows[0]["ModuleId"]);
            module.ModuleName = Convert.ToString(dt.Rows[0]["ModuleName"]);
            return module;
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

        public IEnumerable<Team> GetTeamAll()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Team]";
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
                Team temp = new Team();
                temp.TeamId = Convert.ToInt32(dt.Rows[i]["TeamId"]);
                temp.TeamName = Convert.ToString(dt.Rows[i]["TeamName"]);
                temp.TeamLeader.EmployeeName = Convert.ToString(dt.Rows[i]["TeamLeader"]);
                if (temp.TeamLeader.EmployeeName != "")
                {
                    temp.TeamLeader.EmployeeId = GetEmployeeIdByName(temp.TeamLeader.EmployeeName);
                }
                else
                {
                    temp.TeamLeader.EmployeeId = 0;
                }
                temp.ProjectManager.EmployeeName = Convert.ToString(dt.Rows[i]["ProjectManager"]);
                if (temp.ProjectManager.EmployeeName != "")
                {
                    temp.ProjectManager.EmployeeId = GetEmployeeIdByName(temp.ProjectManager.EmployeeName);
                }
                else
                {
                    temp.ProjectManager.EmployeeId = 0;
                }
                TeamList.Add(temp);
            }
            return TeamList;
        }

        public Team GetTeamById(int id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_Team]";
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
            Team team = new Team();
            team.TeamId = Convert.ToInt32(dt.Rows[0]["TeamId"]);
            team.TeamName = Convert.ToString(dt.Rows[0]["TeamName"]);
            team.TeamLeader.EmployeeName = Convert.ToString(dt.Rows[0]["TeamLeader"]);
            if (team.TeamLeader.EmployeeName != "")
            {
                team.TeamLeader.EmployeeId = GetEmployeeIdByName(team.TeamLeader.EmployeeName);
            }
            else
            {
                team.TeamLeader.EmployeeId = 0;
            }
            team.ProjectManager.EmployeeName = Convert.ToString(dt.Rows[0]["ProjectManager"]);
            if (team.ProjectManager.EmployeeName != "")
            {
                team.ProjectManager.EmployeeId = GetEmployeeIdByName(team.ProjectManager.EmployeeName);
            }
            else
            {
                team.ProjectManager.EmployeeId = 0;
            }
            
            return team;
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
            team.TeamId = Convert.ToInt32(dt.Rows[0]["TeamId"]);
            return team.TeamId;
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
                temp.EmployeeType.TypeId = GetParameterValue("EmployeeType", temp.EmployeeType.TypeName);
                EmployeeList.Add(temp);
            }
            return EmployeeList;
        }
        public IEnumerable<Employee> GetEmployeeByType(int type)
        {
            EmployeeList.Clear();
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
                temp.EmployeeType.TypeId = GetParameterValue("EmployeeType", temp.EmployeeType.TypeName);
                EmployeeList.Add(temp);
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
            employee.EmployeeType.TypeId = GetParameterValue("EmployeeType", employee.EmployeeType.TypeName);
            return employee;
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
            return employee.EmployeeId;
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

        public float GetTotalEpicEstimation()
        {
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
            float TotalEstimation = (float)Convert.ToDouble(dt.Rows[0]["TotalEstimation"]);
            return TotalEstimation;
        }

        public float GetEstimationById(int id)
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_EpicEstimationById]";
                using (SqlCommand sqlCommand = new SqlCommand(procName, sqlConnection))
                {
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@EPICId", id);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            float Estimation = (float)Convert.ToDouble(dt.Rows[0]["Estimation"]);
            return Estimation;
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
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return updatedEmployee;
        }

        public EpicBaseLine UpdateEpicBaseLine(EpicBaseLine updatedEpicBaseLine)
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
                    sqlCommand.Parameters.AddWithValue("@TeamId", updatedEpicBaseLine.TeamName.TeamId);
                    sqlCommand.Parameters.AddWithValue("@IsMurabaha", GetParameterValue("IsMurabaha", updatedEpicBaseLine.IsMurabaha.MurabahaName));
                    sqlCommand.Parameters.AddWithValue("@IsFirstSellableModule", GetParameterValue("IsFirstSellableModule", updatedEpicBaseLine.IsFirstSellableModule.FirstSellableModuleName));
                    sqlCommand.Parameters.AddWithValue("@DistributedUnmappedEffort", updatedEpicBaseLine.DistributedUnmappedEffort);
                    sqlCommand.Parameters.AddWithValue("@TotalActualEffort", updatedEpicBaseLine.TotalActualEffort);
                    sqlConnection.Open();
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        sqlDataAdapter.Fill(dt);
                    }
                }
            }
            return updatedEpicBaseLine;

        }

        public Measurement UpdateMeasurement(Measurement updatedMeasurement)
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
                    sqlCommand.Parameters.AddWithValue("@TeamLeader", updatedTeam.TeamLeader);
                    sqlCommand.Parameters.AddWithValue("@ProjectManager", GetParameterValue("ProjectManagerId", updatedTeam.ProjectManager.EmployeeName));
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

        public List<MeasurementDetailsViewModel> FillMeasurementDetails(int epicId, int year, int month)
        {

            List<MeasurementDetailsViewModel> measurementDetailsList;
            List<Measurement> currentTarget;
            List<Measurement> currentActual;
            List<Measurement> prevActual;
            List<MeasurementSearchModel> searchList;
            float totalEstimation;
            float estimation;


            measurementDetailsList = new List<MeasurementDetailsViewModel>();
            currentTarget = (List<Measurement>)GetMeasurementAll(epicId, year, month, "Target");
            currentActual = (List<Measurement>)GetMeasurementAll(epicId, year, month, "Actual");
            prevActual = (List<Measurement>)GetMeasurementAll(epicId, year, month - 1, "Actual");
            searchList = new List<MeasurementSearchModel>();
            totalEstimation = GetTotalEpicEstimation();

            for (int i = 0; i < currentActual.Count(); i++)
            {
                MeasurementSearchModel temp = new MeasurementSearchModel();
                temp.EpicId = currentActual[i].EpicId;
                temp.Year = currentActual[i].Year;
                temp.Month = currentActual[i].Month;
                searchList.Add(temp);
            }
            for (int i = 0; i < currentTarget.Count(); i++)
            {
                MeasurementSearchModel temp = new MeasurementSearchModel();
                temp.EpicId = currentTarget[i].EpicId;
                temp.Year = currentTarget[i].Year;
                temp.Month = currentTarget[i].Month;
                var x = searchList.Find(m => m.EpicId == temp.EpicId && m.Year == temp.Year && m.Month == temp.Month);
                if (x == null)
                {
                    searchList.Add(temp);
                }
            }
            for (int i = 0; i < searchList.Count(); i++)
            {
                MeasurementDetailsViewModel detailTemp = new MeasurementDetailsViewModel();
                Measurement temp = currentActual.Find(m => (m.EpicId == searchList[i].EpicId || searchList[i].EpicId == 0) && (m.Year == searchList[i].Year || searchList[i].Year == 0) && (m.Month == searchList[i].Month || searchList[i].Month == 0));
                if (temp == null)
                {
                    temp = currentTarget.Find(m => (m.EpicId == searchList[i].EpicId || searchList[i].EpicId == 0) && (m.Year == searchList[i].Year || searchList[i].Year == 0) && (m.Month == searchList[i].Month || searchList[i].Month == 0));
                    
                    detailTemp.ActualRequirementProgress = 0;
                    detailTemp.ActualDesignProgress = 0;
                    detailTemp.ActualDevelopmentProgress = 0;
                    detailTemp.ActualTestProgress = 0;
                    detailTemp.ActualUatProgress = 0;
                }
                else
                {
                    detailTemp.ActualRequirementProgress = temp.RequirementProgress;
                    detailTemp.ActualDesignProgress = temp.DesignProgress;
                    detailTemp.ActualDevelopmentProgress = temp.DevelopmentProgress;
                    detailTemp.ActualTestProgress = temp.TestProgress;
                    detailTemp.ActualUatProgress = temp.UatProgress;
                }
                detailTemp.EpicId = temp.EpicId;
                detailTemp.EpicName = temp.EpicName;
                detailTemp.Module = temp.Module;
                detailTemp.Year = temp.Year;
                detailTemp.Month = temp.Month;
                estimation = GetEstimationById(detailTemp.EpicId);
                detailTemp.Estimation = estimation;
                detailTemp.EpicWeight = estimation / totalEstimation;
                detailTemp.Team = temp.Team;
                detailTemp.ActualOverallEpicCompilation = (float)(detailTemp.ActualRequirementProgress * 0.02 +
                    detailTemp.ActualDesignProgress * 0.23 + detailTemp.ActualDevelopmentProgress * 0.42 +
                    detailTemp.ActualTestProgress * 0.19 + detailTemp.ActualUatProgress * 0.14);
                detailTemp.ActualOverallEpicCompilation = (float)Math.Round(detailTemp.ActualOverallEpicCompilation, 2);
                detailTemp.ActualWeightedOverallProgress = detailTemp.EpicWeight * detailTemp.ActualOverallEpicCompilation;
                detailTemp.PreviousMonthCumulativeActualEffort = temp.PreviousMonthCumulativeActualEffort;
                detailTemp.ActualEffort = temp.ActualEffort;
                temp = currentTarget.Find(m => (m.EpicId == searchList[i].EpicId || searchList[i].EpicId == 0) && (m.Year == searchList[i].Year || searchList[i].Year == 0) && (m.Month == searchList[i].Month || searchList[i].Month == 0));
                detailTemp.TargetRequirementProgress = temp.RequirementProgress;
                detailTemp.TargetDesignProgress = temp.DesignProgress;
                detailTemp.TargetDevelopmentProgress = temp.DevelopmentProgress;
                detailTemp.TargetTestProgress = temp.TestProgress;
                detailTemp.TargetUatProgress = temp.UatProgress;
                detailTemp.TargetOverallEpicCompilation = (float)(detailTemp.TargetRequirementProgress * 0.02 +
                    detailTemp.TargetDesignProgress * 0.23 + detailTemp.TargetDevelopmentProgress * 0.42 +
                    detailTemp.TargetTestProgress * 0.19 + detailTemp.TargetUatProgress * 0.14);
                detailTemp.TargetOverallEpicCompilation = (float)Math.Round(detailTemp.TargetOverallEpicCompilation, 2);
                detailTemp.TargetWeightedOverallProgress = detailTemp.EpicWeight * detailTemp.TargetOverallEpicCompilation;
                temp = prevActual.Find(m => (m.EpicId == searchList[i].EpicId || searchList[i].EpicId == 0) && (m.Year == searchList[i].Year || searchList[i].Year == 0) && (m.Month == searchList[i].Month || searchList[i].Month == 0));
                if (temp != null)
                {
                    detailTemp.PrevMonthRequirementProgress = temp.RequirementProgress;
                    detailTemp.PrevMonthDesignProgress = temp.DesignProgress;
                    detailTemp.PrevMonthDevelopmentProgress = temp.DevelopmentProgress;
                    detailTemp.PrevMonthTestProgress = temp.TestProgress;
                    detailTemp.PrevMonthUatProgress = temp.UatProgress;
                    detailTemp.PrevMonthOverallEpicCompilation = (float)(detailTemp.PrevMonthRequirementProgress * 0.02 +
                        detailTemp.PrevMonthDesignProgress * 0.23 + detailTemp.PrevMonthDevelopmentProgress * 0.42 +
                        detailTemp.PrevMonthTestProgress * 0.19 + detailTemp.PrevMonthUatProgress * 0.14);
                    detailTemp.PrevMonthOverallEpicCompilation = (float)Math.Round(detailTemp.PrevMonthOverallEpicCompilation, 2);
                    detailTemp.PrevMonthWeightedOverallProgress = detailTemp.EpicWeight * detailTemp.PrevMonthOverallEpicCompilation;
                }
                measurementDetailsList.Add(detailTemp);
            }
            return measurementDetailsList;
        }

        public List<Measurement> GenerateMeasurementForNextMonth(int year, int month)
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
                InsertMeasurement(TempMeasurementList[i]);
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

        public int GetMaxModuleId()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_maxModuleId]";
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
            int id = Convert.ToInt32(dt.Rows[0]["MaxModuleId"]);
            return id;
        }
        public int GetMaxTeamId()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_maxTeamId]";
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
            int id = Convert.ToInt32(dt.Rows[0]["MaxTeamId"]);
            return id;
        }
        public int GetMaxEmployeeId()
        {
            DataTable dt = new DataTable();
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                string procName = "[sel_maxEmployeeId]";
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
            int id = Convert.ToInt32(dt.Rows[0]["MaxEmployeeId"]);
            return id;
        }
    }
}
