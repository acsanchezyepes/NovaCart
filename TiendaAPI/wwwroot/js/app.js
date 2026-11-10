'use strict';

const API_URL = '/api';
const STORAGE_KEY = 'novacart.carrito';
const MAX_CANTIDAD = 99;
const money = new Intl.NumberFormat('es-US', { style: 'currency', currency: 'USD' });

const el = {
  products: document.getElementById('products'),
  filters: document.getElementById('filters'),
  catalogStatus: document.getElementById('catalog-status'),
  cartCount: document.getElementById('cart-count'),
  cartContent: document.getElementById('cart-content'),
  cartEmpty: document.getElementById('cart-empty'),
  cartItems: document.getElementById('cart-items'),
  cartTotal: document.getElementById('cart-total'),
  checkout: document.getElementById('checkout'),
  email: document.getElementById('email'),
  checkoutBtn: document.getElementById('checkout-btn'),
  checkoutMsg: document.getElementById('checkout-msg'),
  confirmation: document.getElementById('confirmation'),
  confId: document.getElementById('conf-id'),
  confTotal: document.getElementById('conf-total'),
  confEstado: document.getElementById('conf-estado'),
  confClose: document.getElementById('conf-close'),
  confCheck: document.getElementById('conf-check'),
  confTitulo: document.getElementById('conf-titulo'),
  pago: document.getElementById('pago'),
  pagoOk: document.getElementById('pago-ok'),
  pagoNo: document.getElementById('pago-no'),
  pagoMsg: document.getElementById('pago-msg'),
  toast: document.getElementById('toast'),
};

// Carrito: [{ id, nombre, precio, cantidad }]
let cart = cargarCarrito();
let categoriaActiva = '';

// Orden recién creada y los productos que tenía, por si el pago se rechaza.
let ordenActual = null;
let carritoComprado = [];

/* ---------- Comunicación con la API ---------- */

async function pedirJson(url, opciones = {}) {
  let respuesta;
  try {
    respuesta = await fetch(url, {
      ...opciones,
      headers: { Accept: 'application/json', ...opciones.headers },
    });
  } catch {
    throw new Error('No se pudo conectar con la API. Comprueba que esté en marcha.');
  }

  const texto = await respuesta.text();
  let datos = null;
  if (texto) {
    try { datos = JSON.parse(texto); } catch { datos = texto; }
  }

  if (!respuesta.ok) {
    throw new Error(mensajeDeError(datos, respuesta.status));
  }
  return datos;
}

function mensajeDeError(datos, status) {
  if (typeof datos === 'string' && datos.trim()) return datos;
  if (datos && typeof datos === 'object') {
    if (datos.mensaje) return datos.mensaje;
    if (datos.errors) return Object.values(datos.errors).flat().join(' ');
    if (datos.title) return datos.title;
  }
  return `La API respondió con un error (${status}).`;
}

/* ---------- Catálogo ---------- */

async function cargarProductos(categoria = '') {
  el.catalogStatus.textContent = 'Cargando productos…';
  el.catalogStatus.classList.remove('error');

  const consulta = categoria ? `?categoria=${encodeURIComponent(categoria)}` : '';
  try {
    const productos = await pedirJson(`${API_URL}/productos${consulta}`);
    renderProductos(productos);
    el.catalogStatus.textContent = productos.length ? '' : 'No hay productos en esta categoría.';
    return productos;
  } catch (error) {
    el.products.replaceChildren();
    el.catalogStatus.textContent = error.message;
    el.catalogStatus.classList.add('error');
    return null;
  }
}

function renderProductos(productos) {
  el.products.replaceChildren(...productos.map(crearTarjeta));
}

function crearTarjeta(producto) {
  const li = document.createElement('li');
  li.className = 'card';

  const cuerpo = document.createElement('div');
  cuerpo.className = 'card-body';

  const categoria = document.createElement('span');
  categoria.className = 'card-category';
  categoria.textContent = producto.categoria;

  const nombre = document.createElement('h3');
  nombre.textContent = producto.nombre;

  const descripcion = document.createElement('p');
  descripcion.className = 'card-desc';
  descripcion.textContent = producto.descripcion;

  const pie = document.createElement('div');
  pie.className = 'card-footer';

  const precio = document.createElement('span');
  precio.className = 'price';
  precio.textContent = money.format(producto.precio);

  const boton = document.createElement('button');
  boton.type = 'button';
  boton.className = 'btn primary';
  boton.textContent = '+ Agregar';
  boton.setAttribute('aria-label', `Agregar ${producto.nombre} al carrito`);
  boton.addEventListener('click', () => agregarAlCarrito(producto));

  pie.append(precio, boton);
  cuerpo.append(categoria, nombre, descripcion, pie);
  li.append(crearImagen(producto), cuerpo);
  return li;
}

// Iconos propios (SVG fijos, no vienen de la API).
const ICONOS = {
  teclado: '<rect x="2" y="6" width="20" height="12" rx="2"/><path d="M6 10h.01M10 10h.01M14 10h.01M18 10h.01M6 14h.01M18 14h.01M9 14h6"/>',
  mouse: '<rect x="6.5" y="3" width="11" height="18" rx="5.5"/><path d="M12 7v3.5"/>',
  monitor: '<rect x="3" y="4" width="18" height="12" rx="2"/><path d="M8 20h8M12 16v4"/>',
  audifonos: '<path d="M4 15v-3a8 8 0 0 1 16 0v3"/><rect x="3" y="14" width="4" height="6" rx="1.5"/><rect x="17" y="14" width="4" height="6" rx="1.5"/>',
  microfono: '<rect x="9" y="3" width="6" height="11" rx="3"/><path d="M6 11a6 6 0 0 0 12 0M12 17v4M9 21h6"/>',
  webcam: '<circle cx="12" cy="10" r="6"/><circle cx="12" cy="10" r="2"/><path d="M8 21h8M12 16v5"/>',
  generico: '<path d="M5 8h14l-1.2 12H6.2L5 8z"/><path d="M9 8a3 3 0 0 1 6 0"/>',
};

// Devuelve el icono y el color de fondo según el tipo de producto.
function estiloPara(producto) {
  const texto = `${producto.nombre} ${producto.categoria}`.toLowerCase();
  if (texto.includes('teclado')) return { icono: ICONOS.teclado, tono: 'lavanda' };
  if (texto.includes('mouse') || texto.includes('ratón')) return { icono: ICONOS.mouse, tono: 'menta' };
  if (texto.includes('monitor') || texto.includes('pantalla')) return { icono: ICONOS.monitor, tono: 'cielo' };
  if (texto.includes('audífonos') || texto.includes('auriculares')) return { icono: ICONOS.audifonos, tono: 'durazno' };
  if (texto.includes('micrófono')) return { icono: ICONOS.microfono, tono: 'durazno' };
  if (texto.includes('webcam') || texto.includes('cámara')) return { icono: ICONOS.webcam, tono: 'cielo' };
  return { icono: ICONOS.generico, tono: 'durazno' };
}

function crearIlustracion(producto) {
  const { icono, tono } = estiloPara(producto);
  const div = document.createElement('div');
  div.className = `thumb tono-${tono}`;
  div.setAttribute('aria-hidden', 'true');
  div.innerHTML = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="1.2" stroke-linecap="round" stroke-linejoin="round">${icono}</svg>`;
  return div;
}

// Usa la foto del producto si es real; si es de ejemplo o falla, muestra la ilustración.
function crearImagen(producto) {
  const ilustracion = crearIlustracion(producto);
  const url = producto.imagenUrl || '';
  if (!url || /placeholder/i.test(url)) return ilustracion;

  const img = document.createElement('img');
  img.className = 'thumb';
  img.src = url;
  img.alt = producto.nombre;
  img.loading = 'lazy';
  img.addEventListener('error', () => img.replaceWith(ilustracion), { once: true });
  return img;
}

function renderFiltros(productos) {
  const categorias = [...new Set(productos.map((p) => p.categoria))].sort();
  const opciones = [{ valor: '', texto: 'Todas' }, ...categorias.map((c) => ({ valor: c, texto: c }))];

  el.filters.replaceChildren(...opciones.map(({ valor, texto }) => {
    const boton = document.createElement('button');
    boton.type = 'button';
    boton.className = 'filter';
    boton.textContent = texto;
    boton.setAttribute('aria-pressed', String(valor === categoriaActiva));
    boton.addEventListener('click', () => {
      categoriaActiva = valor;
      for (const b of el.filters.children) b.setAttribute('aria-pressed', 'false');
      boton.setAttribute('aria-pressed', 'true');
      cargarProductos(valor);
    });
    return boton;
  }));
}

/* ---------- Carrito ---------- */

function cargarCarrito() {
  try {
    const datos = JSON.parse(localStorage.getItem(STORAGE_KEY));
    if (!Array.isArray(datos)) return [];
    return datos.filter((i) => Number.isInteger(i.id) && Number.isInteger(i.cantidad) && i.cantidad > 0);
  } catch {
    return [];
  }
}

function guardarCarrito() {
  localStorage.setItem(STORAGE_KEY, JSON.stringify(cart));
}

// Actualiza nombre y precio de lo guardado con los datos actuales del catálogo.
function sincronizarCarrito(productos) {
  const porId = new Map(productos.map((p) => [p.id, p]));
  cart = cart
    .filter((item) => porId.has(item.id))
    .map((item) => ({ ...item, nombre: porId.get(item.id).nombre, precio: porId.get(item.id).precio }));
  guardarCarrito();
}

function agregarAlCarrito(producto) {
  ocultarConfirmacion();
  const item = cart.find((i) => i.id === producto.id);
  if (item) {
    item.cantidad = Math.min(item.cantidad + 1, MAX_CANTIDAD);
  } else {
    cart.push({ id: producto.id, nombre: producto.nombre, precio: producto.precio, cantidad: 1 });
  }
  actualizarCarrito();
  mostrarAviso(`✓ ${producto.nombre} agregado al carrito`);
  animarContador();
}

let avisoTimer;
function mostrarAviso(texto) {
  el.toast.textContent = texto;
  el.toast.classList.add('visible');
  clearTimeout(avisoTimer);
  avisoTimer = setTimeout(() => el.toast.classList.remove('visible'), 2200);
}

function animarContador() {
  el.cartCount.classList.remove('bump');
  void el.cartCount.offsetWidth; // reinicia la animación
  el.cartCount.classList.add('bump');
}

function cambiarCantidad(id, cambio) {
  const item = cart.find((i) => i.id === id);
  if (!item) return;
  item.cantidad += cambio;
  if (item.cantidad <= 0) {
    cart = cart.filter((i) => i.id !== id);
  } else {
    item.cantidad = Math.min(item.cantidad, MAX_CANTIDAD);
  }
  actualizarCarrito();
}

function quitarDelCarrito(id) {
  cart = cart.filter((i) => i.id !== id);
  actualizarCarrito();
}

function actualizarCarrito() {
  guardarCarrito();
  renderCarrito();
}

// Trabaja en centavos para evitar errores de redondeo.
function centavos(precio) {
  return Math.round(precio * 100);
}

function renderCarrito() {
  const unidades = cart.reduce((suma, i) => suma + i.cantidad, 0);
  const totalCentavos = cart.reduce((suma, i) => suma + centavos(i.precio) * i.cantidad, 0);

  el.cartCount.textContent = String(unidades);
  el.cartEmpty.hidden = cart.length > 0;
  el.cartTotal.textContent = money.format(totalCentavos / 100);
  el.cartItems.replaceChildren(...cart.map(crearFilaCarrito));
}

function crearFilaCarrito(item) {
  const li = document.createElement('li');
  li.className = 'cart-item';

  const info = document.createElement('div');
  const nombre = document.createElement('div');
  nombre.className = 'cart-item-name';
  nombre.textContent = item.nombre;
  const unitario = document.createElement('div');
  unitario.className = 'cart-item-unit';
  unitario.textContent = `${money.format(item.precio)} c/u`;
  info.append(nombre, unitario);

  const subtotal = document.createElement('div');
  subtotal.className = 'cart-item-subtotal';
  subtotal.textContent = money.format((centavos(item.precio) * item.cantidad) / 100);

  const controles = document.createElement('div');
  controles.className = 'qty';
  const menos = crearBotonCantidad('−', `Quitar una unidad de ${item.nombre}`, () => cambiarCantidad(item.id, -1));
  const valor = document.createElement('span');
  valor.className = 'qty-value';
  valor.textContent = String(item.cantidad);
  const mas = crearBotonCantidad('+', `Agregar una unidad de ${item.nombre}`, () => cambiarCantidad(item.id, 1));
  mas.disabled = item.cantidad >= MAX_CANTIDAD;
  controles.append(menos, valor, mas);

  const quitar = document.createElement('button');
  quitar.type = 'button';
  quitar.className = 'remove';
  quitar.textContent = 'Quitar';
  quitar.setAttribute('aria-label', `Quitar ${item.nombre} del carrito`);
  quitar.addEventListener('click', () => quitarDelCarrito(item.id));

  li.append(info, subtotal, controles, quitar);
  return li;
}

function crearBotonCantidad(texto, etiqueta, accion) {
  const boton = document.createElement('button');
  boton.type = 'button';
  boton.className = 'btn';
  boton.textContent = texto;
  boton.setAttribute('aria-label', etiqueta);
  boton.addEventListener('click', accion);
  return boton;
}

/* ---------- Compra ---------- */

function mostrarMensaje(texto, tipo = '') {
  el.checkoutMsg.textContent = texto;
  el.checkoutMsg.className = `message ${tipo}`.trim();
}

el.checkout.addEventListener('submit', async (evento) => {
  evento.preventDefault();
  mostrarMensaje('');

  if (cart.length === 0) {
    mostrarMensaje('Agrega al menos un producto al carrito.', 'error');
    return;
  }

  const email = el.email.value.trim();
  if (!email || !el.email.checkValidity()) {
    mostrarMensaje('Escribe un email válido.', 'error');
    el.email.focus();
    return;
  }

  el.checkoutBtn.disabled = true;
  el.checkoutBtn.textContent = 'Procesando…';

  try {
    const orden = await pedirJson(`${API_URL}/ordenes`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        emailCliente: email,
        detalles: cart.map((i) => ({ productoId: i.id, cantidad: i.cantidad })),
      }),
    });

    carritoComprado = cart.map((item) => ({ ...item }));
    cart = [];
    actualizarCarrito();
    el.checkout.reset();
    mostrarConfirmacion(orden);
  } catch (error) {
    mostrarMensaje(error.message, 'error');
  } finally {
    el.checkoutBtn.disabled = false;
    el.checkoutBtn.textContent = 'Comprar';
  }
});

function mostrarConfirmacion(orden) {
  // El total mostrado es el que calculó la API, no el del navegador.
  ordenActual = orden;
  el.confId.textContent = `#${orden.id}`;
  el.confTotal.textContent = money.format(orden.total);
  el.confEstado.textContent = orden.estado;
  mostrarResultado('pendiente', '…', 'Orden creada: falta el pago');
  el.pago.hidden = false;
  el.pagoMsg.textContent = '';
  el.confClose.hidden = true;
  el.confirmation.hidden = false;
  el.cartContent.hidden = true;
  el.pagoOk.focus();
}

function mostrarResultado(tipo, simbolo, titulo) {
  el.confCheck.className = `check ${tipo}`;
  el.confCheck.textContent = simbolo;
  el.confTitulo.className = tipo;
  el.confTitulo.textContent = titulo;
}

async function simularPago(aprobado) {
  if (!ordenActual) return;
  el.pagoOk.disabled = true;
  el.pagoNo.disabled = true;
  el.pagoMsg.className = 'message';
  el.pagoMsg.textContent = 'Procesando pago simulado…';

  try {
    const orden = await pedirJson(`${API_URL}/ordenes/${ordenActual.id}/pago`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ aprobado }),
    });
    ordenActual = orden;
    el.confEstado.textContent = orden.estado;
    el.pago.hidden = true;
    el.confClose.hidden = false;

    if (orden.estado === 'Pagado') {
      mostrarResultado('ok', '✓', '¡Pago aprobado!');
      el.pagoMsg.textContent = 'Gracias por tu compra (simulada).';
      carritoComprado = [];
    } else {
      mostrarResultado('error', '✕', 'Pago rechazado');
      // Devuelve los productos al carrito para poder intentarlo con una nueva orden.
      cart = carritoComprado;
      carritoComprado = [];
      actualizarCarrito();
      el.pagoMsg.textContent = 'Tus productos volvieron al carrito para que lo intentes de nuevo.';
    }
    el.confClose.focus();
  } catch (error) {
    el.pagoMsg.className = 'message error';
    el.pagoMsg.textContent = error.message;
  } finally {
    el.pagoOk.disabled = false;
    el.pagoNo.disabled = false;
  }
}

function ocultarConfirmacion() {
  ordenActual = null;
  el.confirmation.hidden = true;
  el.cartContent.hidden = false;
}

el.pagoOk.addEventListener('click', () => simularPago(true));
el.pagoNo.addEventListener('click', () => simularPago(false));
el.confClose.addEventListener('click', ocultarConfirmacion);

/* ---------- Inicio ---------- */

async function iniciar() {
  renderCarrito();
  const productos = await cargarProductos();
  if (productos) {
    renderFiltros(productos);
    sincronizarCarrito(productos);
    renderCarrito();
  }
}

iniciar();
