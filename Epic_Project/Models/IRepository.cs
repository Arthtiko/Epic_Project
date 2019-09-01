using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epic_Project.Models
{
    public interface IRepository
    {
        #region Team
        IEnumerable<Team> GetTeamAll();
        Team GetTeamById(int id);
        int GetTeamIdByName(string name);
        void DeleteTeam(int id);
        Team InsertTeam(Team team);
        Team UpdateTeam(Team team);
        #endregion

        #region Module
        IEnumerable<Module> GetModuleAll();
        Module GetModuleById(int id);
        void DeleteModule(int id);
        Module InsertModule(Module module);
        Module UpdateModule(Module module);
        #endregion

        #region EpicBaseLine
        IEnumerable<EpicBaseLine> GetEpicBaseLineAll();
        EpicBaseLine GetEpicBaseLineById(int id);
        void DeleteEpicBaseLine(int id);
        EpicBaseLine InsertEpicBaseLine(EpicBaseLine epicBaseLine);
        EpicBaseLine UpdateEpicBaseLine(EpicBaseLine epicBaseLine);

        #endregion

        #region Measurement
        IEnumerable<Measurement> GetMeasurementAll();
        IEnumerable<Measurement> GetMeasurementAll(int epicId, int year, int month, string type);
        void DeleteMeasurement(int epicId, int year, int month, int type);
        Measurement InsertMeasurement(Measurement measurement);
        Measurement UpdateMeasurement(Measurement measurement);
        #endregion

        #region Employee
        IEnumerable<Employee> GetEmployeeAll();
        IEnumerable<Employee> GetEmployeeByType(int type);
        Employee GetEmployeeById(int id);
        void DeleteEmployee(int id);
        Employee InsertEmployee(Employee employee);
        Employee UpdateEmployee(Employee employee);
        #endregion

        int GetParameterValue(string columnName, string parameterName);

        IEnumerable<Parameter> GetParameter(string name);
        float GetTotalEpicEstimation();
        float GetEstimationById(int id);

        List<MeasurementDetailsViewModel> FillMeasurementDetails(int epicId, int year, int month);

        List<Measurement> GenerateMeasurementForNextMonth(int year, int month);
        List<Date> GetDates();
    }
}
