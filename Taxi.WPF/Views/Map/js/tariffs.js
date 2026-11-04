class Tariff{
    constructor(id,title, price, time,imgPath){
        this.id = id;
        this.title = title;
        this.price = price;
        this.time=time;
        this.imgPath=imgPath;
    }
}

const tariffsList = [
  new Tariff(1,'Эконом', 339, '12 мин', 'imgs/classTariff/ekonomClass.png'),
  new Tariff(2,'Комфорт', 450, '7 мин', 'imgs/classTariff/komfort.png'),
  new Tariff(3,'Бизнес', 790, '4 мин', 'imgs/classTariff/business.png'),
  new Tariff(4,'Минивэн', 340, '5 мин', 'imgs/classTariff/miniven.png')
];


function inti_Traffics(){
    const tariffContainer  = document.querySelector(".tariffWrapper");
    console.info(parent);
    tariffsList.forEach(tarif=>{
                const buttonHTML = `
        <button role="radio" aria-pressed="false" aria-checked="false" id="${tarif.id}" class="btnTariffs ${tarif.title == 'Эконом' ? 'active' : ''}">
            <span style="flex-shrink: 0;" class="box--img">
                <img src=${tarif.imgPath} alt="Желтая машина" width="56" height="20" class="car-img">
                <span class="etaTimes">${tarif.time}</span>
            </span>
            <span class="content">
                <span class="title">${tarif.title}</span>
                <span class="price--WnKzJ">
                    <span class="priceSurge--jcKcC"></span>
                    <span class="priceText--YA8Je">от ${tarif.price} ₽</span>
                </span>
            </span>
        </button>
        `;
    
        tariffContainer.insertAdjacentHTML('beforeend', buttonHTML);
    });
    // for (let index = 0; index < tariffsList.length; index++) {

        
    // }

    tariffContainer.addEventListener('click', (event) => {
        const clickedButton = event.target.closest('.btnTariffs');
        if (!clickedButton) return;
        
        selectTariff = tariffsList[ clickedButton.id-1];
        console.info(selectTariff);


        tariffContainer.querySelectorAll('.btnTariffs').forEach(btn =>{
            btn.setAttribute('aria-pressed', 'false');
            btn.setAttribute('aria-checked', 'false');
            btn.classList.remove('active'); 
        });
          clickedButton.setAttribute('aria-pressed', 'true');
        clickedButton.setAttribute('aria-checked', 'true');
        clickedButton.classList.add('active'); 
        
    });
}
