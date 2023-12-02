using Microsoft.AspNetCore.Mvc;
using SistemaVenta.Entity;
using System.Data.SqlClient;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class ClienteController : Controller
    {
        private readonly IConfiguration _configuration;

        public ClienteController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            var facturas = ObtenerFacturasDesdeBD();
            return View(facturas);
        }

        [HttpPost]
        public IActionResult AgregarFactura(string Nombre, string Domicilio, string RFC, DateTime FechaExpedicion)
        {
            GuardarFacturaEnBD(Nombre, Domicilio, RFC, FechaExpedicion);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult GuardarCambiosCliente(int clienteId, string Nombre, string Domicilio, string RFC, DateTime FechaExpedicion)
        {
            GuardarCambios(clienteId, Nombre, Domicilio, RFC, FechaExpedicion);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult EliminarClientePorId(int clienteId)
        {
            bool Error = EliminarCliente(clienteId);
            if (Error == false)
            {
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false});
            }
        }

        [HttpGet]
        public IActionResult ObtenerClientePorId(int clienteId)
        {
            try
            {
                var cliente = ObtenerClientesId(clienteId);

                if (cliente != null)
                {
                    return Json(cliente);
                }

                return NotFound($"Cliente con id {clienteId} no encontrado");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno del servidor: {ex.Message}");
            }
        }

        private void GuardarFacturaEnBD(string Nombre, string Domicilio, string RFC, DateTime FechaExpedicion)
        {
            var connectionString = _configuration.GetConnectionString("CadenaSQL");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var query = "INSERT INTO Cliente (Nombre, Domicilio, RFC, FechaExpedición) VALUES (@Nombre, @Domicilio, @RFC, @FechaExpedicion)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@Nombre", Nombre);
                command.Parameters.AddWithValue("@Domicilio", Domicilio);
                command.Parameters.AddWithValue("@RFC", RFC);
                command.Parameters.AddWithValue("@FechaExpedicion", FechaExpedicion);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private bool EliminarCliente(int clienteId)
        {
            bool Error = false;
            try
            {
                var connectionString = _configuration.GetConnectionString("CadenaSQL");
                using (var connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    var query = "DELETE FROM Cliente WHERE ClienteId = @ClienteId";
                    var command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@ClienteId", clienteId);
                    command.ExecuteNonQuery();
                    connection.Close();
                }
                Error = false;
            }
            catch (Exception ex)
            {
                Error = true;
            }
            return Error;
        }

        private void GuardarCambios(int clienteId, string Nombre, string Domicilio, string RFC, DateTime FechaExpedicion)
        {
            var connectionString = _configuration.GetConnectionString("CadenaSQL");
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var query = "UPDATE Cliente SET Nombre = @Nombre, Domicilio = @Domicilio, RFC = @RFC, FechaExpedición = @FechaExpedicion WHERE ClienteId = @ClienteId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClienteId", clienteId);
                command.Parameters.AddWithValue("@Nombre", Nombre);
                command.Parameters.AddWithValue("@Domicilio", Domicilio);
                command.Parameters.AddWithValue("@RFC", RFC);
                command.Parameters.AddWithValue("@FechaExpedicion", FechaExpedicion);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        private IEnumerable<Cliente> ObtenerClientesId(int clienteId)
        {
            var Cliente = new List<Cliente>();

            var connectionString = _configuration.GetConnectionString("CadenaSQL");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var query = "SELECT ClienteId, Nombre, Domicilio, RFC, FechaExpedición FROM Cliente WHERE ClienteId = @ClienteId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ClienteId", clienteId);

                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var factura = new Cliente
                    {
                        ClienteId = Convert.ToInt32(reader["ClienteId"]),
                        Nombre = Convert.ToString(reader["Nombre"]),
                        Domicilio = Convert.ToString(reader["Domicilio"]),
                        RFC = Convert.ToString(reader["RFC"]),
                        FechaExpedicion = Convert.ToDateTime(reader["FechaExpedición"])
                    };
                    Cliente.Add(factura);
                }
            }
            return Cliente;
        }

        private IEnumerable<Cliente> ObtenerFacturasDesdeBD()
        {
            var facturas = new List<Cliente>();
            var connectionString = _configuration.GetConnectionString("CadenaSQL");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var query = "SELECT ClienteId, Nombre, Domicilio, RFC, FechaExpedición FROM Cliente";
                var command = new SqlCommand(query, connection);
                var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    var factura = new Cliente
                    {
                        ClienteId = Convert.ToInt32(reader["ClienteId"]),
                        Nombre = Convert.ToString(reader["Nombre"]),
                        Domicilio = Convert.ToString(reader["Domicilio"]),
                        RFC = Convert.ToString(reader["RFC"]),
                        FechaExpedicion = Convert.ToDateTime(reader["FechaExpedición"])
                    };
                    facturas.Add(factura);
                }
            }
            return facturas;
        }
    }
}
