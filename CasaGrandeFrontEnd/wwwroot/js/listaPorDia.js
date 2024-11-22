document.addEventListener('DOMContentLoaded', function () {
    // Obteniendo la fecha inicial del atributo 'data-fecha-inicial'
    let initialDateString = document.getElementById('fecha-consultaMedico').getAttribute('data-fecha-inicial');
    let currentMedicoDate = new Date(initialDateString);
    let today = Date.now();

    console.log('Fecha inicial:', currentMedicoDate);

    // Verificar si la fecha es v�lida
    if (isNaN(currentMedicoDate.getTime())) {
        console.error("Fecha inicial no v�lida:", initialDateString);
        return;
    }

    // Selecci�n de elementos y asignaci�n de eventos
    const tecnicoSelect = document.getElementById("tecnico-select");
    const usuarioSelect = document.getElementById("usuario-select");
    const allColumns = Array.from(document.querySelectorAll(".medico-col"));
    const allHeaders = Array.from(document.querySelectorAll("th.col"));

    // Evento para cambiar la visibilidad de las columnas basado en el t�cnico seleccionado
    tecnicoSelect.addEventListener("change", function () {
        const selectedTecnico = tecnicoSelect.value;

        allColumns.forEach(column => {
            // Mostrar todas las columnas si 'all' est� seleccionado
            if (selectedTecnico === "all") {
                column.style.display = "";
            } else {
                // Muestra solo la columna del t�cnico seleccionado y oculta las dem�s
                column.style.display = column.id === `col-${selectedTecnico}` ? "" : "none";
            }
        });

        allHeaders.forEach(header => {
            if (selectedTecnico === "all") {
                header.style.display = ""; // Muestra todos los encabezados
            } else {
                // Muestra solo el encabezado de la columna del t�cnico seleccionado y el de "Hora"
                header.style.display = header.id === `col-${selectedTecnico}` || header.innerText === "Hora" ? "" : "none";
            }
        });
    })
    usuarioSelect.addEventListener("change", function () {
        const usuarioFiltro = usuarioSelect.value.toLowerCase(); // Valor del filtro de usuario
        const filas = document.querySelectorAll("tbody tr"); // Selecciona todas las filas del cuerpo de la tabla

        // Si el filtro es "all", muestra todas las filas, celdas y consultas
        if (usuarioFiltro === "all") {
            filas.forEach(fila => {
                fila.style.display = ""; // Muestra la fila
                const celdas = fila.querySelectorAll("td.medico-col");
                celdas.forEach(celda => {
                    celda.style.display = ""; // Muestra la celda
                    const consultas = celda.querySelectorAll(".consulta-item");
                    consultas.forEach(consulta => {
                        consulta.style.display = ""; // Muestra cada consulta
                    });
                });
            });
            return; // Sale de la funci�n si "all" est� seleccionado
        }
        filas.forEach(fila => {
            let mostrarFila = false;
            const celdas = fila.querySelectorAll("td.medico-col"); 

            celdas.forEach(celda => {
                const consultas = celda.querySelectorAll(".consulta-item"); 
                let mostrarCelda = false; 

                consultas.forEach(consulta => {
                    const nombreUsuario = consulta.querySelector(".nombreUser").innerText.toLowerCase();

                    if (nombreUsuario.includes(usuarioFiltro)) {
                        mostrarCelda = true;  // Marca la celda para mostrarse si coincide el usuario
                        mostrarFila = true;   // Marca la fila para mostrarse si coincide alg�n usuario en ella
                        consulta.style.display = "";  // Muestra la consulta espec�fica si coincide
                    } else {
                        consulta.style.display = "none"; // Oculta la consulta si no coincide
                    }
                });

                celda.style.display = mostrarCelda ? "" : "none"; // Muestra u oculta la celda seg�n el filtro
            });

            fila.style.display = mostrarFila ? "" : "none"; // Muestra u oculta la fila completa seg�n el filtro
        });
    });





        // Actualiza la fecha mostrada inicialmente
        actualizarFecha(currentMedicoDate);

        // Delegaci�n de eventos para los botones de las flechas para m�dicos
    $(document).on("click", "#flecha-izquierda-Medico", function () {
        if (puedeIrAlDiaAnterior()) {
            cambiarFecha(-1); // Restar un d�a
        }
            
        });

        $(document).on("click", "#flecha-derecha-Medico", function () {
            console.log("fecha derecha");
            cambiarFecha(1); // Sumar un d�a
        });




        function cambiarFecha(dias) {
            // Cambiar la fecha actual sumando o restando los d�as
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
        console.error("La fecha no es v�lida:", fechaText);
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
            url: '/Consultas/Edit', // Cambia la URL seg�n sea necesario
            type: 'GET',
            data: consulta, // Pasa todos los atributos como par�metros
            success: function (response) {
                $('#editModalBody').html(response);

                // Intentar buscar un mensaje de �xito en la respuesta
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

    // Cerrar el modal al hacer clic fuera de �l
    $(window).on('click', function (event) {
        if (event.target.id === 'editModal') {
            closeEditModal();
        }
    });

    // Llamada despu�s de la edici�n
    function afterEdit() {
        actualizarConsultas(); // Actualiza la lista de consultas
    }

}
