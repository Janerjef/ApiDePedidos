import { useEffect, useState } from "react";

const API = "http://localhost:5191/api/orders";

const products = [
  { id: "00000000-0000-0000-0000-000000000001", name: "Teclado" },
  { id: "00000000-0000-0000-0000-000000000002", name: "Mouse" },
  { id: "00000000-0000-0000-0000-000000000003", name: "Monitor" },
];

async function request(url, options = {}) {
  const response = await fetch(url, {
    ...options,
    headers: {
      "Content-Type": "application/json",
      ...options.headers,
    },
  });

  if (!response.ok) {
    const message = await response.text();
    throw new Error(`Erro ${response.status}: ${message}`);
  }

  if (response.status === 204) return null;
  return response.json();
}

export default function App() {
  const [orders, setOrders] = useState([]);
  const [details, setDetails] = useState(null);
  const [productId, setProductId] = useState(products[0].id);
  const [quantity, setQuantity] = useState(1);
  const [quantities, setQuantities] = useState({});
  const [error, setError] = useState("");

  async function loadOrders() {
    try {
      setError("");
      setOrders(await request(API));
    } catch (err) {
      setError(err.message);
    }
  }

  useEffect(() => {
    loadOrders();
  }, []);

  async function createOrder(event) {
    event.preventDefault();

    try {
      setError("");
      await request(API, {
        method: "POST",
        body: JSON.stringify({
          items: [{ productId, quantity: Number(quantity) }],
        }),
      });
      await loadOrders();
    } catch (err) {
      setError(err.message);
    }
  }

  async function showDetails(id) {
    try {
      setError("");
      const order = await request(`${API}/${id}`);
      setDetails(order);

      const initialQuantities = {};
      for (const item of order.items) {
        initialQuantities[item.id] = item.quantity;
      }
      setQuantities(initialQuantities);
    } catch (err) {
      setError(err.message);
    }
  }

  async function updateOrder() {
    try {
      setError("");
      await request(`${API}/${details.id}`, {
        method: "PUT",
        body: JSON.stringify({
          items: details.items.map((item) => ({
            itemId: item.id,
            quantity: Number(quantities[item.id]),
          })),
        }),
      });

      await showDetails(details.id);
      await loadOrders();
    } catch (err) {
      setError(err.message);
    }
  }

  async function deleteOrder(id) {
    if (!window.confirm("Deseja remover este pedido?")) return;

    try {
      setError("");
      await request(`${API}/${id}`, { method: "DELETE" });
      if (details?.id === id) setDetails(null);
      await loadOrders();
    } catch (err) {
      setError(err.message);
    }
  }

  return (
      <main style={{ maxWidth: 800, margin: "30px auto", fontFamily: "Arial" }}>
        <h1>Pedidos</h1>

        {error && <p style={{ color: "red" }}>{error}</p>}

        <h2>Criar pedido</h2>
        <form onSubmit={createOrder}>
          <label>
            Produto{" "}
            <select
                value={productId}
                onChange={(event) => setProductId(event.target.value)}
            >
              {products.map((product) => (
                  <option key={product.id} value={product.id}>
                    {product.name}
                  </option>
              ))}
            </select>
          </label>{" "}
          <label>
            Quantidade{" "}
            <input
                type="number"
                min="1"
                value={quantity}
                onChange={(event) => setQuantity(event.target.value)}
            />
          </label>{" "}
          <button type="submit">Criar pedido</button>
        </form>

        <h2>Pedidos cadastrados</h2>
        <button onClick={loadOrders}>Atualizar lista</button>

        <ul>
          {orders.map((order) => (
              <li key={order.id} style={{ margin: "12px 0" }}>
                Pedido {order.id} — {order.status} — Total: {order.totalAmount}{" "}
                <button onClick={() => showDetails(order.id)}>Detalhes</button>{" "}
                <button onClick={() => deleteOrder(order.id)}>Remover</button>
              </li>
          ))}
        </ul>

        {details && (
            <section>
              <h2>Detalhes do pedido</h2>
              <p>ID: {details.id}</p>
              <p>Data: {details.orderDate}</p>
              <p>Status: {details.status}</p>

              {details.items.map((item) => (
                  <div key={item.id} style={{ margin: "10px 0" }}>
                    {item.productName} — preço unitário: {item.unitPrice} —{" "}
                    <label>
                      Quantidade{" "}
                      <input
                          type="number"
                          min="1"
                          value={quantities[item.id] ?? item.quantity}
                          onChange={(event) =>
                              setQuantities({
                                ...quantities,
                                [item.id]: event.target.value,
                              })
                          }
                      />
                    </label>
                  </div>
              ))}

              <p>Total: {details.totalAmount}</p>
              <button onClick={updateOrder}>Salvar quantidades</button>
              <button onClick={() => setDetails(null)}>Fechar detalhes</button>
            </section>
        )}
      </main>
  );
}