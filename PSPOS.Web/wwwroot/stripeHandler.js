let stripe = null;
let card;

export async function initializeStripe(publishableKey) {
    if (!stripe) {
        console.log("Initializing Stripe with key:", publishableKey);
        stripe = Stripe(publishableKey);
        const elements = stripe.elements();
        card = elements.create("card", {
            style: { base: { fontSize: "16px", color: "#32325d" } },
        });
        card.mount("#card-element");
    }
}

export async function confirmPayment(clientSecret) {
    return new Promise(async (resolve) => {
        const { error, paymentIntent } = await stripe.confirmCardPayment(clientSecret, {
            payment_method: { card: card },
        });

        if (error) {
            console.error("Payment failed:", error.message);
            resolve({ success: false, message: error.message });
        } else {
            console.log("Payment succeeded:", paymentIntent.id);
            resolve({ success: true, id: paymentIntent.id });
        }
    });
}
