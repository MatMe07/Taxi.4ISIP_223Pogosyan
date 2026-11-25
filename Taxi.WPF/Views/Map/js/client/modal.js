class PaymentMethod{
    constructor(id, name, pathIcon){
        this.id = id; 
        this.name = name;
        this.pathIcon = pathIcon;

    }
}
function btnClickConfrim() {
    console.info(addressOtkuda);
    console.info(addressKuda);
    console.info(selectTariff)
    if (addressKuda === null) {
        ShowSnackbar("Укажите, куда вы едете");
        clientElements.searchInput2.focus();
        clientElements.searchInput2.classList.add('input-error');
        return;
    }
    postToHost({
            "type": 'createNewOrder', 
            // "addressKuda":addressKuda,
            // "addressOtkuda":addressOtkudacreateNewOrder,
            "selectTariff":selectTariff.id.toString(),
            "paymentId":selectedPaymentMethod.toString(),
            "paymentAmount": selectTariff.price.toString()
        });
}

function init_RBPaymentMethod() {
    const PaymentMethodListElement = clientElements.paymentList;
    // console.info(PaymentMethodList);    

    // PaymentMethodList.childNodes.forEach(method=>{   
    //     // console.info(method);
    // });

    PaymentMethodListElement.addEventListener('click', (event) => {
        const clickedRadio = event.target.closest('.payment-row');
        // console.info(clickedRadio);

        PaymentMethodListElement.querySelectorAll('.payment-row').forEach(radioBtn => {
            radioBtn.classList.remove("selected");
        });
        clickedRadio.classList.add("selected");
        selectedPaymentMethod = clickedRadio.dataset.method;
        // console.info(outputFieldPayMethod);
        clientElements.methodPayment.textContent = PaymentMethodList.find(p=>p.id==selectedPaymentMethod).name;
        // console.info
        postToHost({type:'changePaymentMethod',selectedPaymentMethodId: selectedPaymentMethod});
        closeModel();
    });
}

function UpdateRBPaymentMethod() {
    const btnPaymentMethod = clientElements.btnPaymentMethod;
    btnPaymentMethod.style.display = "block";
    const PaymentMethodListElement = clientElements.paymentList;
    var innerHtml = ``;
    var ClassSelected = "";
    PaymentMethodList.forEach(element => {
        console.info(element);
        console.info(selectedPaymentMethod);
        if (selectedPaymentMethod === element.id){
            // console.info(selectedPaymentMethod);
            ClassSelected = "selected";
        }
        else {
            ClassSelected = "";
        }
        innerHtml += `
                    <div class="payment-row ${ClassSelected}" data-method="${element.id}">
                        <div class="payment-info">
                            <img src="${element.pathIcon}" alt="${element.name}" class="payment-icon">
                            <span class="payment-name">${element.name}</span>
                        </div>
                        <div class="custom-radio"></div>
                    </div>
        `;
        clientElements.methodPayment.textContent = PaymentMethodList.find(p=>p.id==selectedPaymentMethod).name;

        // console.info(innerHtml);
    });
    PaymentMethodListElement.innerHTML = innerHtml;
}


function btnModelOpen() {
    clientElements.modal.classList.add("open");
}

function closeModel() {
    clientElements.modal.classList.remove("open");
    console.info(selectedPaymentMethod);
}

function ShowSnackbar(message) {
    var x = clientElements.snackbar;
    x.innerHTML = `<span>${message}</span>`;
    x.className = "show";
    setTimeout(function () { x.className = x.className.replace("show", ""); }, 3000);
}


function btnModalWaitOpen(){
    clientElements.modalWait.classList.add("open");
}

function btnCloseModalWait() {
    clientElements.modalWait.classList.remove("open");
    // console.info(selectedPaymentMethod);
}

function closeModalWaiting(){
    clientElements.orderScreen.style.display = 'block';
    clientElements.waitingScreen.style.display = 'none';
    clientElements.modalWait.classList.remove("open");
    clearAnim();
    showTariffsSkeleton();
    postToHost({type:'cancelOrder', orderId: CurrentOrder.toString()});
    postToHost({type:'updateTariffs'});
}
