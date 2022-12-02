var datatable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable() {
    datatable = $('#tblDatos').DataTable({
        "language": {
            "decimal": "",
            "emptyTable": "No hay información",
            "info": "Mostrando _START_ a _END_ de _TOTAL_ Entradas",
            "infoEmpty": "Mostrando 0 de 0 de 0 Entradas",
            "infoFiltered": "(Filtrado de _MAX_ total entradas)",
            "infoPostFix": "",
            "thousands": ",",
            "ordering": "true",
            "lengthMenu": "Mostrar _MENU_ Entradas",
            "loadingRecords": "Cargando...",
            "processing": "Procesando...",
            "search": "Buscar:",
            "zeroRecords": "Sin resultados encontrados",
            "paginate": {
                "first": "Primero",
                "last": "Ultimo",
                "next": "Siguiente",
                "previous": "Anterior"
            }
        },
        "ajax": {
            "url": "/Admin/Usuario/obtenerTodos"
        },
        "columns": [
            { "data": "userName", "width": "10%" },
            { "data": "nombres", "width": "10%" },
            { "data": "apellidos", "width": "10%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "10%" },
            { "data": "rol", "width": "15%" },

            {
                "data": {
                    id:"id", lockoutEnd:"lockoutEnd"
                },
                "render": function (data) {

                    var hoy = new Date().getTime();
                    var bloqueo = new Date(data.lockoutEnd).getTime();
                    if (bloqueo > hoy) {

                        // Ususuario está bloqueado
                        return `
                        <div class="text-center">
                            <a onclick=BloquearDesbloquear('${data.id}') class="btn btn-danger text-white" style="cursor:pointer; width:150px;">
                                <i class="fas fa-lock-open"></i> Desbloquear
                            </a>
                        </div>
                        `;
                    }
                    else {
                        return `
                        <div class="text-center">
                            <a onclick=BloquearDesbloquear('${data.id}') class="btn btn-success text-white" style="cursor:pointer; width:150px;">
                                <i class="fas fa-lock"></i> Bloquear
                            </a>
                        </div>
                        `;
                    }
                   
                }, "width": "20%"
            }
        ]
    });
}

function BloquearDesbloquear(id) {
   
    $.ajax({
        type: "POST",
        url: '/Admin/Usuario/BloquearDesbloquear',
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                datatable.ajax.reload();
            } else {
                toastr.error(data.message);
            }
        }
    });
}