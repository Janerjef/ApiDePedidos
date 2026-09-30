namespace ApiDePedidos.Domain;

public enum OrderStatus
{
    AguardandoPagamento = 1,
    Aprovado,
    EmSeparacao,
    Enviado,
    Entregue,
    Cancelado,
    Reembolsado
}