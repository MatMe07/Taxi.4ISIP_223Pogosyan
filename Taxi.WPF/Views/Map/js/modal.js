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
        input_seach2.focus();
        input_seach2.classList.add('input-error');
        return;
    }

}

function init_RBPaymentMethod() {
    const PaymentMethodListElement = document.querySelector(".payment-list");
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
        console.info(outputFieldPayMethod);
        outputFieldPayMethod.textContent = PaymentMethodList.find(p=>p.id==selectedPaymentMethod).name;
        // console.info
        postToHost({type:'changePaymentMethod',selectedPaymentMethodId: selectedPaymentMethod});
        closeModel();
    });
}

function UpdateRBPaymentMethod() {
    const btnPaymentMethod = document.querySelector(".btnPaymentMethod");
    btnPaymentMethod.style.display = "block";
    const PaymentMethodListElement = document.querySelector(".payment-list");
    var innerHtml = ``;
    var ClassSelected = "";
    PaymentMethodList.forEach(element => {
        if (selectedPaymentMethod === element.id){

            console.info(selectedPaymentMethod);
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
        outputFieldPayMethod.textContent = PaymentMethodList.find(p=>p.id==selectedPaymentMethod).name;

        // console.info(innerHtml);
    });
    PaymentMethodListElement.innerHTML = innerHtml;
}


function btnModelOpen() {
    modelWindow.classList.add("open");
}

function closeModel() {
    modelWindow.classList.remove("open");
    console.info(selectedPaymentMethod);
}

function ShowSnackbar(message) {
    var x = document.getElementById("snackbar");
    x.innerHTML = `<span>${message}</span>`;
    x.className = "show";
    setTimeout(function () { x.className = x.className.replace("show", ""); }, 3000);
}
