function debounce(func, delay) {
  let timeout;
  return function (...args) {
    clearTimeout(timeout);
    timeout = setTimeout(() => func.apply(this, args), delay);
  };
}



// ymaps.ready(init);
init();


function postToHost(message) {
    if (window.chrome && window.chrome.webview) {
        window.chrome.webview.postMessage(JSON.stringify(message));
    } else {
        console.log('[-> C#]', JSON.stringify(message));
    }
}

function showNotice(text) {
    var notice = document.getElementById('notice');
    notice.textContent = text;
    notice.classList.add('visible');
    document.getElementById('search').classList.add('input_error');
}

function hideNotice(elementId) {
    var notice = document.getElementById('notice');
    notice.textContent = '';
    notice.classList.remove('visible');
    document.getElementById(elementId).classList.remove('input_error');
}

function getUserLocation() {
    // postToHost({type:"getUserLocation"});
}

function setOtkudaInput(){
    if (!input_seach1.value.trim()){
        input_seach1.value = addressOtkuda;
    }
    else{
        if (input_seach1.value !== addressOtkuda){
            input_seach1.value = addressOtkuda;
        }
    }
}

function setupSearchInput(input, placeholder, getAddress, onBlurExtra) {
    input.addEventListener('focus', () => {
        if (input.value.trim() !== '') {
            placeholder.classList.add('hidden');
        }
    });

    input.addEventListener('input', () => {
        if (input.value.trim() !== '') {
            placeholder.classList.add('hidden');
        } else {
            placeholder.classList.remove('hidden');
        }
    });

    input.addEventListener('blur', () => {
        const value = input.value.trim();
        const address = getAddress();

        if (value && input.value !== address) {
            input.value = address;
        }

        if (value !== '') {
            placeholder.classList.remove('hidden');
            placeholder.classList.add('active');
        } else {
            placeholder.classList.remove('active', 'hidden');
        }

        if (typeof onBlurExtra === 'function') {
            onBlurExtra();
        }
    });
}
