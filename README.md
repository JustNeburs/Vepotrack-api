# Vepotrack-api
> API de posicionamiento de vehiculos

Esta API es una prueba para ver que funciones serían necesarias para permitir tener posicionados los pedidos para los clientes.

## Consideraciones
+ Se usa .Net Core 2.1
+ Se ha decidido usar EF Core en memoría pues solo se usa para pruebas y no necesita persistencia
+ Nada más iniciar se generan
  + *Roles de usuario*:
   + **Admin**: Administrador con todos los permisos
   + **Vehicle**: Usuario responsable de un vehículo
   + **Regular**: Usuario estandar de una web/app movil
  + *Usuarios*: 
   + **Admin**: Usuario con rol de administrador y password **Ad.123456**
   + **Vehicle01**: Usuario con rol de vehículo y password **Ve.123456**
   + **Regular01**: Usuario con rol estadnar y password **Re.123456**
+ Todos los objetos, vehiculos, pedidos, ... tienen un Id interno (*GUID*) pero se obtienen/asignan por una referencia de texto para hacerlos compatibles con otros sistemas.  
+ Para pruebas se ha generado una colección de Postman y algunas pruebas unitarias. 
## Login
> /api/login/authenticate

Se ha incluido una autenticación para obtener un token para permitir acceder al resto de opciones.  
## Pedidos
> /api/order

Los pedidos tienen una Referencia para acceder a ellos y una Referencia Única de Acceso para permitir acceder a su estado sin tener que hacer login en el sistema. 
### Creación
Para crear pedidos tendremos que indicar al menos, la referencia y referencia única. 
Para poder añadir pedidos hay que ser administrador o responsable de vehículo.
Los pedidos nada más crearlos, si no se ha indicado lo contrario tienen el estado de Added. 
### Estados
Los posibles estados son:
+ **None**: Desconocido 
+ **Added**: Añadido al sistema
+ **Assigned**: Asignado a vehículo
+ **InCourse**: En ruta
+ **Delviery**: Entregado
+ **Warning**: Problema
+ **Cancel**: Cancelado
### Opciones de asignación o actualización
A los pedidos se pueden actualizar y se les puede indicar la referencia del vehículo que los tiene asignados; y el usuario del sistema que puede ver el estado del pedido (Además del administrador).  
Por seguridad se guarda un historial de los cambios que sufren los pedidos, aunque de momento no es accesible por la API.
Además de haber un servicio web para actualizar pedidos, también hay un servicio web que asigna el pedido al vehiculo solo indicando sus referencias. 
### Obtener pedido
Cuando se obtiene un pedido (pasando la referencia) se obtendrán los datos del pedido, así como su estado en formato de texto, la referencia del vehículo que lo lleva y la última posición conocida de este.

## Vehículos
> /api/vehicle

Los vehículos tienen un referencia que debe ser única. Además de permitir crear, actualizar, listar y obtener vehículos, se puede indicar la última posición de un vehículo así como obtener las posiciones de hoy de un vehículo indicado.
### Creación
Para la creación de un vehículo (solo administrador) se ha de indicar la referencia de este, un nombre representativo y la matrícula. 
Pudiendo indicar también el nombre del usuario que tiene asignado el vehículo.  
### Actualizar posición
Llamando al servicio web que permite establecer la ubicación del vehículo se dará de alta una entrada de posición. Para saber la posición se han de pasar obligatoriamente **Latitud**, **Longitud** y **Precisión**, así como la fecha/hora de la posición. Además se puede pasar a modo informativo una indicación de formato e información extendida.   
### Obtención 
Cuando se obtiene un vehículo se obtendrá la referencia, el nombre, matrícula, conductor (en caso de tener uno asignado) y las referencias de los pedidos que actualmente lleva como asignados o en ruta. 
### Listar posiciones
Se permite listar las posiciones de un vehículo pasando la referncia de este. Solo se ha publicado un servicio web para obtener las del día de hoy pero ya esta preparado para obtenr un listado de posiciones entre fechas y paginado. 

# Ampliaciones
Se ha incluido servicios web para la notificación de posiciones, se dará la posibilidad de notificar tanto por Webhook como por MQTT, aunque este último solo esta planteado y preparado para instalar el cliente. 
Todas las notificaciones tienen una fecha de expiración que habrá que renovar, para un usuario regular, si se apunta a una notificación a los 5 minutos dejará de recibir las notificaciones, para un vehículo dejará de recibirlas pasado medio día y para un administrador pasado 1 día completo.

## Webhook
> /order/notify/webhook/{referenceOrder}

Permitirá añadir una notificación por Webhook, debiendo pasar en el body la URL a notificar. 

## MQTT
> /order/notify/mqtt/{referenceOrder}

### Funcionamiento actual 

Permitirá añadir una notificación por MQTT la cual, actualmente, no notificará nada. 

### Planteamiento

Se deberá configurar el servidor de MQTT y se notificará a una cola para el usuario que la pida y el vehículo indicado ../UserName/ReferenceVehicle 
