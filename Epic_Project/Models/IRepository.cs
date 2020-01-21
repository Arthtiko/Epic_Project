using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public interface IRepository
    {
        #region Team
        IEnumerable<Team> GetTeamAll(int id, string name, int teamLeaderId, int projectManagerId);
        void DeleteTeam(int id);
        Team InsertTeam(Team team);
        Team UpdateTeam(Team team);
        #endregion

        #region Module
        IEnumerable<Module> GetModuleAll();
        List<Module> GetModuleProgress(int year, int month);
        void DeleteModule(int id);
        Module InsertModule(Module module);
        Module UpdateModule(Module module);
        #endregion

        #region EpicBaseLine
        IEnumerable<EpicBaseLine> GetEpicBaseLineAll(int id);
        void DeleteEpicBaseLine(int id, string userName, string ipAddress);
        EpicBaseLine InsertEpicBaseLine(EpicBaseLine epicBaseLine, string userName, string ipAddress);
        EpicBaseLine UpdateEpicBaseLine(EpicBaseLine epicBaseLine, string userName, string ipAddress);

        #endregion

        #region Measurement
        IEnumerable<Measurement> GetMeasurementAll(int epicId, int year, int month, string type);
        void DeleteMeasurement(int epicId, int year, int month, int type, string userName, string ipAddress);
        Measurement InsertMeasurement(Measurement measurement, string userName, string ipAddress);
        Measurement UpdateMeasurement(Measurement measurement, string userName, string ipAddress);
        IEnumerable<MeasurementLogModel> GetLogMeasurement(int epicId, int year, int month, string type, string user);
        #endregion

        #region Employee
        IEnumerable<Employee> GetEmployeeAll();
        IEnumerable<Employee> GetEmployeeByType(int type);
        Employee GetEmployeeById(int id);
        void DeleteEmployee(int id);
        Employee InsertEmployee(Employee employee);
        Employee UpdateEmployee(Employee employee);
        #endregion

        #region Date Control
        IEnumerable<DateControl> GetDateControl(int? year, int? month, string type);
        DateControl InsertDateControl(DateControl dateControl);
        void DeleteDateControl(int year, int month);
        DateControl UpdateDateControl(DateControl dateControl);
        #endregion

        #region Finance
        IEnumerable<Finance> GetFinanceAll(string category, int year, int month);
        IEnumerable<FinanceReport> GetFinanceReport(string category, int year, int month);
        Finance InsertFinanceAll(Finance finance);
        void DeleteFinance(Finance finance);
        Finance UpdateFinanceAll(Finance finance);
        List<Date> GetFinanceDates();
        void GenerateNewFinanceMonth();
        void DeleteLastFinanceMonth();
        IEnumerable<FinanceGraph> GetFinanceGraph(bool isTotal);
        #endregion

        #region Parameter
        int GetParameterValue(string columnName, string parameterName);
        IEnumerable<Parameter> GetParameter(string name);
        #endregion

        #region Progress
        ProgressModel GetProgress(int year, int month, string location, string isFirstSellableModule);
        IEnumerable<LineChartModel> GetLineChartProgress(string location, string isFirstSellableModule, string type);
        IEnumerable<HighLevelProgress> GetHighLevelProgress(string location, string isFirstSellableModule, Date date);
        IEnumerable<HighLevelProgress> GetHighLevelProgressNew(string location, string isFirstSellableModule, Date date);
        #endregion

        #region Identity
        int GetEmployeeId(string userId);
        #endregion

        #region Measurement Operations
        List<MeasurementDetailsViewModel> FillMeasurementDetails(int year, int month, string location, string isFSM, string team);
        List<Measurement> GenerateMeasurementForNextMonth(int year, int month, string location, string userName, string ipAddress);
        void DeleteLastMonth(int month, int year, string location, string userName, string ipAddress);
        List<Measurement> SearchMeasurement(int year, int month, string location, string type, string teamName);
        #endregion

        #region Side Operations
        List<Date> GetDates();
        float GetEpicWeight(string location, string isFirstSellableModule);
        List<int> GetEpicBaseLineIdByLocation(string location);
        #endregion

        IEnumerable<MeasurementLogModel> mahmut();
    }
}
