$(document).ready(function () {
    // Inicializar el datepicker
    $('#datepicker').datepicker({
        format: 'yyyy-mm-dd',
        autoclose: true,
        todayHighlight: true
    });

    // Escuchar cuando cambia la fecha
    $('#datepicker').on('changeDate', function (e) {
        var selectedDate = e.format(); // Obtener la fecha seleccionada en formato yyyy-mm-dd

        // Llamar a la función para cargar las consultas
        cargarConsultasParaFecha(selectedDate);
    });

    // Función para cargar las consultas para una fecha específica
    function cargarConsultasParaFecha(fecha) {
        $.ajax({
            url: '/Consultas/cancelarConsultaXfecha', // Endpoint en tu controlador
            type: 'GET',
            data: { fecha: fecha }, // Pasar la fecha seleccionada como parámetro
            success: function (data) {
                $('#divCancelarConsultas').html(data); // Actualizar el div con la tabla de consultas
            },
            error: function (xhr, status, error) {
                console.error("Error al cargar las consultas:", error);
            }
        });
    }
});