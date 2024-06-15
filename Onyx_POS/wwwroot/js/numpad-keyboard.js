const Keyboard = {
    elements: {
        main: null,
        keysContainer: null,
        keys: [],
        closeButton: null
    },

    eventHandlers: {
        oninput: null,
        onclose: null
    },

    properties: {
        value: "",
        capsLock: false,
        currentPad: "num"
    },

    init() {
        // Create main elements
        var currentPad = Keyboard.properties.currentPad;
        this.elements.main = document.createElement("div");
        this.elements.keysContainer = document.createElement("div");
        this.elements.toggleButton = document.createElement("button");
        this.elements.closeButton = document.createElement("button");
        // Setup main elements  
        var className = currentPad == "num" ? "numpad" : "keyboard";
        this.elements.main.classList.add(className, "keyboard--hidden");
        if (currentPad)
            this.elements.main.classList.add("numpad");
        this.elements.keysContainer.classList.add("keyboard__keys");

        this.elements.toggleButton.classList.add("toggle__keypad");
        var icon = currentPad == "num" ? "number" : "keyboard";
        this.elements.toggleButton.innerHTML = `<img src="/assets/media/kb-icon/${icon}.png" alt="Close" style="height: 30px;">`;
        this.elements.toggleButton.addEventListener("click", () => {
            Keyboard.close();

            Keyboard.properties.currentPad = currentPad == "num" ? "keyboard" : "num";
            Keyboard.init();
            setTimeout(function () {
                Keyboard.open();
            })
        });

        this.elements.closeButton.classList.add("keyboard__close");
        this.elements.closeButton.innerHTML = `<img src="/assets/media/kb-icon/close.svg" alt="Close" style="height: 10px;">`;
        this.elements.closeButton.addEventListener("click", () => {
            this.close();
        });

        this.elements.keysContainer.appendChild(this._createKeys());

        this.elements.keys = this.elements.keysContainer.querySelectorAll(".keyboard__key");

        // Add to DOM
        this.elements.main.appendChild(this.elements.keysContainer);
        this.elements.main.appendChild(this.elements.toggleButton);
        this.elements.main.appendChild(this.elements.closeButton);
        document.body.appendChild(this.elements.main);

        // Automatically use keyboard for elements with .use-keyboard-input
        document.querySelectorAll(".keyboard-input").forEach(element => {
            element.addEventListener("focus", () => {
                this.open(element.value, currentValue => {
                    element.value = currentValue;
                });
            });
        });

        document.addEventListener("click", (event) => {
            if (!this.elements.main.contains(event.target) && !event.target.classList.contains("keyboard-input")) {
                this.close();
            }
        });
    },

    _createKeys() {
        const fragment = document.createDocumentFragment();
        var currentPad = Keyboard.properties.currentPad;
        const keyLayout = currentPad == "num" ?
            [
                "1", "2", "3",
                "4", "5", "6",
                "7", "8", "9",
                "0", "backspace", "done"
            ] :
            [
                "q", "w", "e", "r", "t", "y", "u", "i", "o", "p",
                "a", "s", "d", "f", "g", "h", "j", "k", "l",
                "caps", "z", "x", "c", "v", "b", "n", "m", "backspace",
                "space", "done"
            ];
        // Creates HTML for an icon
        const createIconHTML = (icon_name) => {
            var iconHtml = icon_name == 'backspace' || icon_name == 'keyboard_capslock' || icon_name == 'keyboard_return' ? `<img src="/assets/media/kb-icon/${icon_name}.svg" style="height: 25px" />` : `<span class="fs-4">${icon_name}</span>`;
            return iconHtml;
        };

        keyLayout.forEach(key => {
            const keyElement = document.createElement("button");

            const insertLineBreak = currentPad == "num" ?
                ["3", "6", "9"].indexOf(key) !== -1 :
                ["0", "p", "l", "backspace"].indexOf(key) !== -1;

            // Add attributes/classes
            keyElement.setAttribute("type", "button");
            keyElement.classList.add("keyboard__key");
            if (currentPad == "num")
                keyElement.classList.add("numpad__key");
            switch (key) {
                case "backspace":
                    if (currentPad != "num")
                        keyElement.classList.add("keyboard__key--wide");
                    keyElement.innerHTML = createIconHTML("backspace");

                    keyElement.addEventListener("click", () => {
                        this.properties.value = this.properties.value.substring(0, this.properties.value.length - 1);
                        this._triggerEvent("oninput");
                    });

                    break;

                case "caps":
                    keyElement.classList.add("keyboard__key--wide", "keyboard__key--activatable");
                    keyElement.innerHTML = createIconHTML("keyboard_capslock");

                    keyElement.addEventListener("click", () => {
                        this._toggleCapsLock();
                        keyElement.classList.toggle("keyboard__key--active", this.properties.capsLock);
                    });

                    break;

                case "enter":
                    keyElement.classList.add("keyboard__key--wide");
                    keyElement.innerHTML = createIconHTML("keyboard_return");

                    keyElement.addEventListener("click", () => {
                        this.properties.value += "\n";
                        this._triggerEvent("oninput");
                    });

                    break;

                case "space":
                    keyElement.classList.add("keyboard__key--extra-wide");
                    keyElement.innerHTML = createIconHTML("Space");

                    keyElement.addEventListener("click", () => {
                        this.properties.value += " ";
                        this._triggerEvent("oninput");
                    });

                    break;

                case "done":
                    keyElement.classList.add("bg-primary");
                    keyElement.classList.add("text-white");
                    if (currentPad != "num")
                        keyElement.classList.add("keyboard__key--wide");
                    keyElement.innerHTML = createIconHTML("Done");
                    keyElement.addEventListener("click", () => {
                        this.close();
                        this._triggerEvent("onclose");
                    });

                    break;

                default:
                    keyElement.textContent = key.toLowerCase();

                    keyElement.addEventListener("click", () => {
                        this.properties.value += this.properties.capsLock ? key.toUpperCase() : key.toLowerCase();
                        this._triggerEvent("oninput");
                    });

                    break;
            }

            fragment.appendChild(keyElement);

            if (insertLineBreak) {
                fragment.appendChild(document.createElement("br"));
            }
        });

        return fragment;
    },

    _triggerEvent(handlerName) {
        if (typeof this.eventHandlers[handlerName] == "function") {
            this.eventHandlers[handlerName](this.properties.value);
        }
    },

    _toggleCapsLock() {
        this.properties.capsLock = !this.properties.capsLock;

        for (const key of this.elements.keys) {
            if (key.childElementCount === 0) {
                key.textContent = this.properties.capsLock ? key.textContent.toUpperCase() : key.textContent.toLowerCase();
            }
        }
    },

    open(initialValue, oninput, onclose) {
        this.properties.value = initialValue || "";
        this.eventHandlers.oninput = oninput;
        this.eventHandlers.onclose = onclose;
        this.elements.main.classList.remove("keyboard--hidden");
    },

    close() {
        this.properties.value = "";
        this.eventHandlers.oninput = oninput;
        this.eventHandlers.onclose = onclose;
        this.elements.main.classList.add("keyboard--hidden");
    }
};

window.addEventListener("DOMContentLoaded", function () {
    Keyboard.init();
});
