function btnClickConfrim(){
    console.info(addressOtkuda);
    console.info(addressKuda);
    console.info(selectTariff)
    
}

function init_RBPaymentMethod(){
    PaymentMethodList = document.querySelector(".payment-list");
    // console.info(PaymentMethodList);    

    // PaymentMethodList.childNodes.forEach(method=>{
    //     // console.info(method);

        
    // });

    PaymentMethodList.addEventListener('click', (event) => {
        const clickedRadio = event.target.closest('.payment-row');
        // console.info(clickedRadio);
        
        PaymentMethodList.querySelectorAll('.payment-row').forEach(radioBtn=>{
            radioBtn.classList.remove("selected");

            
        });
        clickedRadio.classList.add("selected");
        selectedPaymentMethod = clickedRadio.dataset.method;
        outputFieldPayMethod.textContent = paymentNames[selectedPaymentMethod];
        closeModel();


    });
}


function btnModelOpen(){
    modelWindow.classList.add("open");
}

function closeModel(){
    modelWindow.classList.remove("open");
    console.info(selectedPaymentMethod);
}
