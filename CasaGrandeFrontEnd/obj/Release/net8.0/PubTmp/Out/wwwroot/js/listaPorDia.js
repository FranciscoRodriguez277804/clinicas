document.addEventListener('DOMContentLoaded', function () {
    // Obteniendo la fecha inicial del atributo 'data-fecha-inicial'
    let initialDateString = document.getElementById('fecha-consultaMedico').getAttribute('data-fecha-inicial');
    let currentMedicoDate = new Date(initialDateString);
    let today = Date.now();

    console.log('Fecha inicial:', currentMedicoDate);

    // Verificar si la fecha es válida
    if (isNaN(currentMedicoDate.getTime())) {
        console.error("Fecha inicial no válida:", initialDateString);
        return;
    }

    // Selección de elementos y asignación de eventos
    const tecnicoSelect = document.getElementById("tecnico-select");
    const allColumns = Array.from(document.querySelectorAll(".medico-col"));
    const allHeaders = Array.from(document.querySelectorAll("th.col"));

    // Evento para cambiar la visibilidad de las columnas basado en el técnico seleccionado
    tecnicoSelect.addEventListener("change", function () {
        const selectedTecnico = tecnicoSelect.value;

        allColumns.forEach(column => {
            // Mostrar todas las columnas si 'all' está seleccionado
            if (selectedTecnico === "all") {
                column.style.display = "";
            } else {
                // Muestra solo la columna del técnico seleccionado y oculta las demás
                column.style.display = column.id === `col-${selectedTecnico}` ? "" : "none";
            }
        });

        allHeaders.forEach(header => {
            if (selectedTecnico === "all") {
                header.style.display = ""; // Muestra todos los encabezados
            } else {
                // Muestra solo el encabezado de la columna del técnico seleccionado y el de "Hora"
                header.style.display = header.id === `col-${selectedTecnico}` || header.innerText === "Hora" ? "" : "none";
            }
        });
    })


        // Actualiza la fecha mostrada inicialmente
        actualizarFecha(currentMedicoDate);

        // Delegación de eventos para los botones de las flechas para médicos
    $(document).on("click", "#flecha-izquierda-Medico", function () {
        if (puedeIrAlDiaAnterior()) {
            cambiarFecha(-1); // Restar un día
        }
            
        });

        $(document).on("click", "#flecha-derecha-Medico", function () {
            console.log("fecha derecha");
            cambiarFecha(1); // Sumar un día
        });




        function cambiarFecha(dias) {
            // Cambiar la fecha actual sumando o restando los días
            currentMedicoDate.setDate(currentMedicoDate.getDate() + dias);

            // Formatear la nueva fecha
            var nuevaFechaFormateada = currentMedicoDate.toISOString().split('T')[0]; // Formato "YYYY-MM-DD"

            // Actualizar el atributo data y el texto del elemento con la nueva fecha
            var fechaElemento = document.getElementById("fecha-consultaMedico");
            fechaElemento.setAttribute("data-fecha-inicial", nuevaFechaFormateada);
            fechaElemento.textContent = nuevaFechaFormateada;
            console.log(nuevaFechaFormateada);

            // Recargar la tabla de consultas llamando al backend
            cargarConsultasParaFecha(nuevaFechaFormateada);
        }

        function cargarConsultasParaFecha(fecha) {
            $.ajax({
                url: '/Consultas/ListarConsultasPorDia',
                type: 'GET',
                data: { fecha: fecha },
                success: function (data) {
                    $('#tablePorDia').html(data);
                },
                error: function (xhr, status, error) {
                    console.error("Error al cargar las consultas:", error);
                }
            });
        }

        function actualizarFecha(fecha) {
            // Actualiza el elemento con la fecha inicial en el formato deseado
            document.getElementById('fecha-consultaMedico').textContent = fecha.toISOString().split('T')[0];
    }
    function puedeIrAlDiaAnterior() {
        return currentMedicoDate => today;
    }
    });
    
function agregarConsulta(element) {
    var fechaText = $(element).data('fecha');
    var fecha = new Date(fechaText);

    if (!isNaN(fecha.getTime())) {
        var formattedFecha = fecha.toISOString().split('T')[0]; // Formato ISO para la fecha
    } else {
        console.error("La fecha no es válida:", fechaText);
        return;
    }

    var hora = $(element).data('hora');
    var medico = $(element).data('medico');

    $.ajax({
        url: '/Consultas/AgregarConsultaPartial',
        type: 'GET',
        data: {
            fecha: formattedFecha,
            hora: hora,
            tecnico: medico
        },
        success: function (data) {
            $('#AgregarModal').html(data);
            $('#consultaAgregarModal').show();
        },
        error: function (xhr, status, error) {
            console.error('Error al cargar el formulario de consulta:', error);
        }
    });
    
    function showEditForm(consulta) {
        console.log("hola");
        console.log('consulta:', consulta); // Muestra todos los atributos

        $.ajax({
            url: '/Consultas/Edit', // Cambia la URL según sea necesario
            type: 'GET',
            data: consulta, // Pasa todos los atributos como parámetros
            success: function (response) {
                $('#editModalBody').html(response);

                // Intentar buscar un mensaje de éxito en la respuesta
                var mensaje = $('#editModalBody').find('.alert-info').text();
                if (mensaje) {
                    alert(mensaje);
                    $('#editModal').hide(); // Oculta el modal si el mensaje es exitoso
                }
            },
            error: function () {
                alert('Error al cargar la vista de pagos');
            }
        });
    }




    function closeEditModal() {
        $('#editModal').hide(); // Oculta el modal
        $('#editModalBody').html(''); // Limpia el contenido del modal
    }

    // Cerrar el modal al hacer clic fuera de él
    $(window).on('click', function (event) {
        if (event.target.id === 'editModal') {
            closeEditModal();
        }
    });

    // Llamada después de la edición
    function afterEdit() {
        actualizarConsultas(); // Actualiza la lista de consultas
    }

}
