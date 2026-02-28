# Vlys Scene Notes 1.0.2

**Vlys Scene Notes** te permite crear, organizar y gestionar notas contextuales directamente dentro del Scene View de Unity, mejorando la comunicación del equipo y la documentación a nivel de escena.

---

## Primeros Pasos

1. Abre el **Scene View**.
2. Localiza el botón de overlay **“SN”**.
3. Haz clic en **Scene Notes**.
4. Presiona **Create Note**.
5. Haz clic en cualquier parte de la escena para colocar la nota
   *(Puedes reposicionarla más adelante.)*
6. Selecciona la nota en el Inspector.
7. Escribe el contenido y presiona **Save**.

---

## Edición de Notas

* Selecciona una nota existente.
* Haz clic en **Enable Editing**.
* Modifica:

  * Texto
  * Categoría
  * Estado
  * Posición
* Presiona **Apply Changes** para confirmar.

---

## Sistema de Comentarios

Cada nota permite agregar comentarios que incluyen:

* Autor
* Timestamp automático
* Contexto de cambios

Esto facilita el seguimiento del historial de comunicación directamente vinculado a una ubicación específica dentro de la escena.

---

## Gestión de Notas

* Usa los filtros del overlay en el Scene View para controlar qué categorías son visibles.
* Elimina notas individuales desde el Inspector.
* Para eliminar todas las notas de la escena actual, ve a:

```
Vlys → Scene Notes → Delete All Notes
```

Aparecerá un cuadro de confirmación antes de la eliminación permanente.

---

## Personalización Visual

Puedes personalizar completamente cómo se ven las notas tanto en el **Scene View** como en el **Inspector**.

Dirígete a:

```
Vlys → Scene Notes → Style Settings
```

Desde allí puedes modificar:

* Colores
* Tipografía
* Apariencia del layout
* Estados visuales

Los cambios se aplican inmediatamente en todo el proyecto.

---

## Configuración de Iconos por Defecto

Cada categoría admite iconos personalizados.

Para modificar los iconos por defecto, abre:

```
Vlys Notes/DefaultSettings/SceneNoteIconConfig.asset
```

Desde este asset puedes asignar diferentes iconos a cada categoría según las necesidades de tu flujo de trabajo.

---

## Características Principales

* Notas contextuales dentro de la escena
* Filtros por categoría
* Seguimiento de estado
* Historial de comentarios con autor y fecha
* Personalización visual completa
* Iconos configurables por categoría
* Protección contra pérdida de cambios no guardados

---

Diseñado para optimizar la comunicación y la claridad de producción directamente dentro de tus escenas de Unity.
