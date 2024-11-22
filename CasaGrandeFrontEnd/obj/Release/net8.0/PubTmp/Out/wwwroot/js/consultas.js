function verDetalles(element) {
    // Encuentra el div.menuHover más cercano al elemento de "Ver Detalles"
    const menu = element.closest(".consulta-item").querySelector(".menuHover");
    
    // Alterna la visibilidad del menú
    if (menu.style.display === "none" || menu.style.display === "") {
        menu.style.display = "block";
    } else {
        menu.style.display = "none";
    }
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

function closeModal() {
    var modal = document.getElementById("myModal");
    modal.style.display = "none";

}
function closeAgregarModal() {
    var modal = document.getElementById("consultaAgregarModal");
    modal.style.display = "none";
}
function closePagoModal() {
    var modal = document.getElementById("pagoModal");
    model.style.display = "none";
}


$('#buscadorTecnicoFiltro').on('input', function () {
    var term = $(this).val().trim();

    if (term.length > 0) {
        $.ajax({
            url: '/Consultas/BuscarTecnico',
            data: { term: term },
            success: function (data) {
                displayTechnicianSuggestions(data);
            },
            error: function () {
                console.error('Error al buscar técnico');
            }
        });
    } else {
        $('#sugerenciasTecnicoFiltro').empty();
    }
});

$('#buscadorUsuarioFiltro').on('input', function () {
    var term = $(this).val().trim();

    if (term.length > 0) {
        $.ajax({
            url: '/Consultas/BuscarUsuario',
            data: { term: term },
            success: function (data) {
                displayUserSuggestions(data);
            },
            error: function () {
                console.error('Error al buscar usuario');
            }
        });
    } else {
        $('#sugerenciasUsuarioFiltro').empty();
    }
});

function displayTechnicianSuggestions(suggestions) {
    var $suggestionsContainer = $('#sugerenciasTecnicoFiltro');
    $suggestionsContainer.empty();

    if (suggestions.length > 0) {
        $suggestionsContainer.addClass('active'); // Muestra el contenedor
    } else {
        $suggestionsContainer.removeClass('active'); // Oculta si no hay sugerencias
    }

    suggestions.forEach(function (item) {
        var suggestion = $("<button></button>")
            .addClass("btn-sm suggestion-btn btn-light")
            .text(item.primerNombre + " " + item.primerApellido)
            .attr("data-id", item.id);

        suggestion.on("click", function () {
            $('#buscadorTecnicoFiltro').val(item.primerNombre + " " + item.primerApellido);
            $('#buscadorTecnicoFiltro').attr("data-id", item.id);
            $suggestionsContainer.empty();
            $suggestionsContainer.removeClass('active'); // Oculta las sugerencias después de seleccionar
        });

        $suggestionsContainer.append(suggestion);
    });
}
function displayUserSuggestions(suggestions) {
    var $suggestionsContainer = $('#sugerenciasUsuarioFiltro');
    $suggestionsContainer.empty();

    if (suggestions.length > 0) {
        $suggestionsContainer.addClass('active'); // Muestra el contenedor
    } else {
        $suggestionsContainer.removeClass('active'); // Oculta si no hay sugerencias
    }

    suggestions.forEach(function (item) {
        var suggestion = $("<button></button>")
            .addClass("btn-sm suggestion-btn btn-light")
            .text(item.primerNombre + " " + item.primerApellido)
            .attr("data-id", item.id);

        suggestion.on("click", function () {
            $('#buscadorUsuarioFiltro').val(item.primerNombre + " " + item.primerApellido);
            $('#buscadorUsuarioFiltro').attr("data-id", item.id);
            $suggestionsContainer.empty();
            $suggestionsContainer.removeClass('active'); // Oculta las sugerencias después de seleccionar
        });

        $suggestionsContainer.append(suggestion);
    });
}

function filtrarPorSucursal() {
    var sucursal = document.getElementById("sucursalSelect").value;
    var filas = document.querySelectorAll("#tablePorDia tbody tr");

    filas.forEach(function (fila) {
        fila.querySelectorAll('td ul li[data-sucursal]').forEach(function (consulta) {
            // Mostrar u ocultar cada celda según la sucursal seleccionada
            if (sucursal === "todas" || consulta.getAttribute("data-sucursal") === sucursal) {
                consulta.style.display = "";  // Mostrar la celda
            } else {
                consulta.style.display = "none";  // Ocultar la celda
            }
        });
    });
}
function actualizarConsultas() {
    $.ajax({
        url: '/Consultas/ListarConsultasPorDia', // URL que apunta al método ListarConsultasPorDia
        type: 'GET',
        success: function (response) {
            $('#tablePorDia').html(response); // Actualiza el contenido de la vista parcial
            $('#tablePorMedico').hide();
            $('#filterPanel1').hide();
        },
        error: function (xhr, status, error) {
            console.error("Error al actualizar la lista de consultas: " + xhr.responseText);
        }
    });
}
function actualizarConsultasPorMedico() {
    $.ajax({
        url: '/Consultas/ListarConsultasPorMedico', // URL que apunta al método ListarConsultasPorDia
        type: 'GET',
        success: function (response) {
            $('#tablePorMedico').html(response); // Actualiza el contenido de la vista parcial
            $('#tablePorMedico').show(); // Asegúrate de mostrar la tabla por médico
            $('#filterPanel1').hide(); // Oculta el panel de filtros si es necesario
        },
        error: function (xhr, status, error) {
            console.error("Error al actualizar la lista de consultas: " + xhr.responseText);
        }
    });
}



function openPagoModal() {
    $('#pagoModal').show();
    $('.modal-backdrop').addClass('show');
}

function closePagoModal() {
    $('#pagoModal').hide();
    $('.modal-backdrop').addClass('hidde');
}
function openModal() {
    $('#myModal').css('display', 'block');
    $('.modal-backdrop').addClass('show');
}

function closeModal() {
    $('#myModal').css('display', 'none');
}


$(document).ready(function () {
    // Delegación de eventos para los elementos que puedan ser reemplazados
    $(document).on('click', '#consultasConPagosPendientes', function (e) {
        e.preventDefault();
        console.log("Botón clickeado");
        $.ajax({
            type: 'GET',
            url: '/Consultas/ListarConsultasPendientes',
            success: function (response) {
                $('#tablePendientes').html(response);
                $('#tablePendientes').show();
                $('#tablePorMedico').hide();
                $('#tablePorDia').hide();
                $('#tablaFiltro').hide();
                $('#filterPanel1').hide();
                $('#DivtablaxMedico').hide();
            },
            error: function (xhr, status, error) {
                console.error(xhr.responseText);
            }
        });
    });
    $(document).ready(function () {
        // Delegación de eventos para los elementos que puedan ser reemplazados
        $(document).on('click', '#btnListarConsultasPorMedico', function (e) {
            e.preventDefault();
            console.log("Botón clickeado");
            $.ajax({
                type: 'GET',
                url: '/Consultas/ListarConsultasPorMedico',
                success: function (response) {
                    $('#tablePorMedico').html(response);
                    $('#tablePorMedico').show();
                    $('#tablePorDia').hide();
                    $('#filterPanel1').hide();
                    $('#tablaFiltro').hide();
                    $('#DivtablaxMedico').show();
                    $('#tablePendientes').hide();
                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                }
            });
        });

        $(document).on('click', '#consultasDelDia', function (e) {
            e.preventDefault();
            console.log("Botón clickeado");
            $.ajax({
                type: 'GET',
                url: '/Consultas/ListarConsultasPorDia',
                success: function (response) {
                    $('#tablePorDia').html(response);
                    $('#tablePorDia').show();
                    $('#tablePorMedico').hide();
                    $('#tablaFiltro').hide();
                    $('#filterPanel1').hide();
                    $('#tablePendientes').hide();
                },
                error: function (xhr, status, error) {
                    console.error(xhr.responseText);
                }
            });
        });
        $(document).on('click', '#filtrarPorFecha', function (e) {
            e.preventDefault();
            console.log("Botón de filtrar clickeado");
            $('#filterPanel1').show();
            $('#filter').show();
            $('#tablePorDia').hide();
            $('#tablaFiltro').show();
            $('#tablePendientes').hide();
            $('#tablePorMedico').hide();
        });

        // Manejador para la casilla de verificación de rango de fechas
        $('#toggleDateRange').on('change', function () {
            if (this.checked) {
                $('#dateConsulta').hide();
                $('#dateRangeFields').show();
                $('#dateStart').show();
                $('#dateEnd').show();
                $('#labelStart').show();
                $('#labelEnd').show();
                $('#labelDate').hide();
            } else {
                $('#dateConsulta').show();
                $('#labelDate').show();
                $('#dateStart').hide();
                $('#dateRangeFields').hide();
                $('#dateEnd').hide();
                $('#labelStart').hide();
                $('#labelEnd').hide();
            }
        });
    });
});
$(document).ready(function () {
    // Manejo del envío del formulario de filtro
    $('#filterForm').on('submit', function (e) {
        e.preventDefault(); // Evita que el formulario recargue la página

        var formData = $(this).serialize(); // Obtiene los datos del formulario

        $.ajax({
            url: '/Consultas/Filtrado', // Asegúrate de que esta URL esté correcta
            type: 'POST',
            data: formData,
            success: function (response) {
                $('#tablaFiltro').html(response); // Actualiza la vista con los resultados filtrados
                $('#tablaFiltro').show();
                $('#tablePorDia').hide();
                $('#tablePorMedico').hide();
                $('#tablePendientes').hide();
            },
            error: function (xhr, status, error) {
                console.error('Error al aplicar el filtro: ' + xhr.responseText);
            }
        });
    });
});
document.querySelectorAll('.menuItem').forEach(item => {
    item.addEventListener('click', function () {
        document.querySelectorAll('.menuItem').forEach(link => link.classList.remove('active'));
        this.classList.add('active');
    });
});