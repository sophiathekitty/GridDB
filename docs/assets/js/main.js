// Null Systems Corporate Website - Interactive JavaScript
// "Where functionality meets the void"

document.addEventListener('DOMContentLoaded', function() {
    initializeNullSystems();
});

function initializeNullSystems() {
    console.log('🧊 Initializing Null Systems interface...');
    console.log('📡 GridDB Framework loading...');
    console.log('✨ Redefining absence as a service...');
    
    // Initialize all systems
    initializeMusic();
    initializeAnimatedCounters();
    initializeGlitchEffects();
    initializeRandomQuotes();
    initializeFakeLogin();
    initializeMatrixRain();
    initializeConsoleMessages();
    initializeFormHandlers();
    initializeEasterEggs();
    
    console.log('✅ Null Systems fully operational (probably)');
}

// ========== BACKGROUND MUSIC SYSTEM ==========
function initializeMusic() {
    const musicToggle = document.getElementById('music-toggle');
    const backgroundMusic = document.getElementById('background-music');
    let isPlaying = false;
    
    if (!musicToggle || !backgroundMusic) return;
    
    musicToggle.addEventListener('click', function() {
        if (isPlaying) {
            backgroundMusic.pause();
            musicToggle.textContent = '♪ SYNTHWAVE';
            musicToggle.style.color = 'var(--primary-pink)';
            console.log('🎵 Audio transcendence paused');
        } else {
            // Note: Autoplay policies require user interaction
            backgroundMusic.play().catch(e => {
                console.log('🎵 Audio enlightenment blocked by browser policy');
            });
            musicToggle.textContent = '♪ PLAYING';
            musicToggle.style.color = 'var(--accent-green)';
            console.log('🎵 Entering audio transcendence mode');
        }
        isPlaying = !isPlaying;
    });
}

// ========== ANIMATED COUNTERS ==========
function initializeAnimatedCounters() {
    const counters = document.querySelectorAll('.stat-number');
    const observerOptions = {
        threshold: 0.5
    };
    
    const observer = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                animateCounter(entry.target);
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);
    
    counters.forEach(counter => {
        observer.observe(counter);
    });
}

function animateCounter(element) {
    const targetText = element.getAttribute('data-target');
    const currentText = element.textContent;
    const duration = 2000; // 2 seconds
    const startTime = performance.now();
    
    // Parse the target value to determine type and extract number
    let targetNumber, suffix, decimals, isSpecial, startValue;
    
    if (targetText === 'UNLIMITED') {
        // Special case for UNLIMITED
        isSpecial = true;
        const letters = 'UNLIMITED'.split('');
        let currentIndex = 0;
        
        function animateUnlimited() {
            if (currentIndex < letters.length) {
                element.textContent = letters.slice(0, currentIndex + 1).join('');
                currentIndex++;
                setTimeout(animateUnlimited, 100);
            }
        }
        
        element.textContent = '';
        animateUnlimited();
        console.log('📊 Counter optimized: UNLIMITED');
        return;
    }
    
    // Extract number and suffix for other formats
    const targetMatch = targetText.match(/^(\d+(?:\.\d+)?)(.*)$/);
    const currentMatch = currentText.match(/^(\d+(?:\.\d+)?)(.*)$/);
    
    if (targetMatch) {
        targetNumber = parseFloat(targetMatch[1]);
        suffix = targetMatch[2];
        decimals = (targetMatch[1].includes('.')) ? targetMatch[1].split('.')[1].length : 0;
        
        // Determine start value - if current text has a number, use it, otherwise start from 0
        if (currentMatch) {
            startValue = parseFloat(currentMatch[1]);
        } else {
            startValue = 0;
        }
    } else {
        console.error('Invalid target format:', targetText);
        return;
    }
    
    function updateCounter(currentTime) {
        const elapsed = currentTime - startTime;
        const progress = Math.min(elapsed / duration, 1);
        
        // Add some quantum uncertainty to our numbers
        const randomFactor = Math.random() * 0.02 - 0.01; // ±1%
        const easeOutQuart = 1 - Math.pow(1 - progress, 4);
        let currentValue = startValue + (targetNumber - startValue) * easeOutQuart;
        
        // Format the number based on original format
        let displayValue;
        if (decimals > 0) {
            displayValue = currentValue.toFixed(decimals);
        } else {
            displayValue = Math.floor(currentValue).toString();
        }
        
        element.textContent = displayValue + suffix;
        
        if (progress < 1) {
            requestAnimationFrame(updateCounter);
        } else {
            element.textContent = targetText;
            console.log(`📊 Counter optimized: ${targetText}`);
        }
    }
    
    requestAnimationFrame(updateCounter);
}

// ========== ENHANCED GLITCH EFFECTS ==========
function initializeGlitchEffects() {
    const glitchElements = document.querySelectorAll('.glitch');
    
    glitchElements.forEach(element => {
        // Random glitch activation
        setInterval(() => {
            if (Math.random() < 0.05) { // 5% chance every interval
                triggerGlitch(element);
            }
        }, 3000);
        
        // Hover glitch effect
        element.addEventListener('mouseenter', () => {
            triggerGlitch(element);
        });
    });
}

function triggerGlitch(element) {
    element.style.animation = 'none';
    element.offsetHeight; // Trigger reflow
    element.style.animation = 'glitch-1 0.3s, glitch-2 0.3s';
    
    setTimeout(() => {
        element.style.animation = '';
    }, 300);
}

// ========== RANDOM CORPORATE QUOTES ==========
function initializeRandomQuotes() {
    const quotes = [
        "Optimizing your reality framework...",
        "Synchronizing with the void...",
        "Redefining absence parameters...",
        "Calculating optimal nothingness...",
        "Transcending data limitations...",
        "Achieving peak efficiency through elimination...",
        "Processing structured emptiness...",
        "Implementing next-generation void solutions...",
        "Harmonizing with absence protocols...",
        "Revolutionizing nothing, perfectly."
    ];
    
    // Show random quotes in console
    setInterval(() => {
        const randomQuote = quotes[Math.floor(Math.random() * quotes.length)];
        console.log(`💭 ${randomQuote}`);
    }, 15000); // Every 15 seconds
}

// ========== FAKE LOGIN SYSTEM ==========
function initializeFakeLogin() {
    // Create floating login prompt occasionally
    setInterval(() => {
        if (Math.random() < 0.1) { // 10% chance
            createFakeLoginPrompt();
        }
    }, 30000); // Every 30 seconds
}

function createFakeLoginPrompt() {
    const loginDiv = document.createElement('div');
    loginDiv.innerHTML = `
        <div style="
            position: fixed;
            top: 20px;
            right: 20px;
            background: var(--secondary-bg);
            border: 2px solid var(--primary-cyan);
            padding: 1rem;
            border-radius: 10px;
            z-index: 1001;
            color: var(--text-primary);
            font-family: var(--font-primary);
            font-size: 0.9rem;
            box-shadow: var(--glow-cyan);
            animation: fadeInOut 4s ease-in-out;
        ">
            <div style="margin-bottom: 0.5rem; color: var(--primary-cyan);">
                🔐 SECURE LOGIN DETECTED
            </div>
            <div style="color: var(--text-secondary); font-size: 0.8rem;">
                Access level: TRANSCENDENT<br>
                Status: Optimally secured
            </div>
        </div>
    `;
    
    document.body.appendChild(loginDiv);
    
    setTimeout(() => {
        if (loginDiv.parentNode) {
            loginDiv.parentNode.removeChild(loginDiv);
        }
    }, 4000);
    
    console.log('🔐 Secure login event simulated');
}

// ========== MATRIX RAIN BACKGROUND ==========
function initializeMatrixRain() {
    // Only on homepage
    if (!document.querySelector('.hero')) return;
    
    const canvas = document.createElement('canvas');
    const ctx = canvas.getContext('2d');
    
    canvas.style.position = 'fixed';
    canvas.style.top = '0';
    canvas.style.left = '0';
    canvas.style.width = '100%';
    canvas.style.height = '100%';
    canvas.style.pointerEvents = 'none';
    canvas.style.zIndex = '1';
    canvas.style.opacity = '0.1';
    
    document.body.appendChild(canvas);
    
    function resizeCanvas() {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
    }
    
    resizeCanvas();
    window.addEventListener('resize', resizeCanvas);
    
    const chars = '01NULLGRIDDBVOID∅⚡🧊🖥️';
    const charArray = chars.split('');
    const fontSize = 14;
    const columns = canvas.width / fontSize;
    const drops = Array(Math.floor(columns)).fill(1);
    
    function drawMatrix() {
        ctx.fillStyle = 'rgba(10, 10, 10, 0.05)';
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        
        ctx.fillStyle = '#00ffff';
        ctx.font = `${fontSize}px monospace`;
        
        for (let i = 0; i < drops.length; i++) {
            const text = charArray[Math.floor(Math.random() * charArray.length)];
            ctx.fillText(text, i * fontSize, drops[i] * fontSize);
            
            if (drops[i] * fontSize > canvas.height && Math.random() > 0.975) {
                drops[i] = 0;
            }
            drops[i]++;
        }
    }
    
    setInterval(drawMatrix, 100);
}

// ========== CONSOLE EASTER EGGS ==========
function initializeConsoleMessages() {
    // Welcome message
    setTimeout(() => {
        console.log('%c🧊 GRIDDB FRAMEWORK LOADED', 'color: #00ffff; font-size: 16px; font-weight: bold;');
        console.log('%cWelcome to the void. Your data is safe here (probably).', 'color: #ff00ff;');
        console.log('%cType "null.help()" for hidden commands', 'color: #888;');
    }, 1000);
    
    // Make null object available globally for easter eggs
    window.null = {
        help: function() {
            console.log('%cNULL SYSTEMS HIDDEN COMMANDS:', 'color: #00ffff; font-weight: bold;');
            console.log('%cnull.transcend() - Achieve data enlightenment', 'color: #ff00ff;');
            console.log('%cnull.optimize() - Optimize current page for maximum void', 'color: #ff00ff;');
            console.log('%cnull.status() - Check system status', 'color: #ff00ff;');
            console.log('%cnull.coffee() - Emergency caffeine protocol', 'color: #ff00ff;');
        },
        transcend: function() {
            console.log('%c✨ TRANSCENDENCE INITIATED...', 'color: #ff8800; font-size: 18px;');
            document.body.style.animation = 'transcendence 3s ease-in-out';
            setTimeout(() => {
                document.body.style.animation = '';
                console.log('%c🌟 You have achieved data enlightenment.', 'color: #00ff88;');
            }, 3000);
        },
        optimize: function() {
            console.log('%c⚡ OPTIMIZING INTERFACE FOR MAXIMUM VOID...', 'color: #ffff00;');
            const elements = document.querySelectorAll('*');
            elements.forEach(el => {
                if (Math.random() < 0.01) { // 1% chance
                    el.style.opacity = '0.8';
                    setTimeout(() => el.style.opacity = '', 2000);
                }
            });
            console.log('%c✅ Optimization complete. Reality adjusted by 0.7%', 'color: #00ff88;');
        },
        status: function() {
            console.log('%cSYSTEM STATUS REPORT:', 'color: #00ffff; font-weight: bold;');
            console.log(`CPU Usage: ${Math.floor(Math.random() * 15)}%`);
            console.log(`Memory: ${Math.floor(Math.random() * 30)}% of infinity`);
            console.log(`Void Levels: ${Math.floor(Math.random() * 100)}% optimal`);
            console.log(`Coffee Status: ${Math.random() > 0.5 ? 'CRITICAL' : 'Adequate'}`);
            console.log(`CEO Location: [REDACTED]`);
            console.log(`Reality Stability: ${Math.floor(Math.random() * 20 + 80)}%`);
        },
        coffee: function() {
            console.log('%c☕ EMERGENCY CAFFEINE PROTOCOL ACTIVATED', 'color: #8B4513; font-weight: bold;');
            console.log('%cDispensing virtual coffee...', 'color: #8B4513;');
            setTimeout(() => {
                console.log('%c☕ Coffee delivered. Productivity increased by 247%', 'color: #00ff88;');
            }, 2000);
        }
    };
}

// ========== FORM HANDLERS ==========
function initializeFormHandlers() {
    // Contact form
    const contactForm = document.querySelector('.contact-form');
    if (contactForm) {
        contactForm.addEventListener('submit', handleContactSubmit);
    }
    
    // Newsletter form
    const newsletterForm = document.querySelector('.newsletter-form');
    if (newsletterForm) {
        newsletterForm.addEventListener('submit', handleNewsletterSubmit);
    }
}

function handleContactSubmit(event) {
    event.preventDefault();
    
    const submitBtn = event.target.querySelector('.submit-btn');
    const statusDiv = document.getElementById('form-status');
    
    // Show loading state
    submitBtn.classList.add('loading');
    submitBtn.disabled = true;
    
    const responses = [
        "Your message has been optimized for maximum efficiency and stored in the void.",
        "Thank you for contacting the void. The void acknowledges your existence.",
        "Message received and transcended. Response time: Eventually.",
        "Your inquiry has been forwarded to our philosophy department.",
        "Message successfully converted to structured absence.",
        "Thank you. Your communication has achieved perfect optimization (it no longer exists)."
    ];
    
    setTimeout(() => {
        const randomResponse = responses[Math.floor(Math.random() * responses.length)];
        
        statusDiv.innerHTML = `
            <div style="
                background: var(--accent-bg);
                border: 1px solid var(--accent-green);
                color: var(--accent-green);
                padding: 1rem;
                border-radius: 5px;
                margin-top: 1rem;
            ">
                ✅ ${randomResponse}
            </div>
        `;
        
        submitBtn.classList.remove('loading');
        submitBtn.disabled = false;
        
        // Reset form after a delay
        setTimeout(() => {
            event.target.reset();
            statusDiv.innerHTML = '';
        }, 5000);
        
        console.log('📧 Message successfully nullified');
    }, 2000);
}

function handleNewsletterSubmit(event) {
    event.preventDefault();
    
    const email = event.target.querySelector('input[type="email"]').value;
    const button = event.target.querySelector('button');
    
    button.textContent = 'OPTIMIZING...';
    button.disabled = true;
    
    setTimeout(() => {
        button.textContent = 'SUBSCRIBED TO VOID';
        button.style.background = 'var(--accent-green)';
        
        console.log(`📧 Email ${email} successfully subscribed to structured absence`);
        
        setTimeout(() => {
            button.textContent = 'SUBSCRIBE';
            button.disabled = false;
            button.style.background = '';
            event.target.reset();
        }, 3000);
    }, 1500);
}

// ========== GENERAL INTERACTIVE ELEMENTS ==========
function goNowhere() {
    console.log('🚀 Initiating journey to nowhere...');
    
    const button = event.target;
    const originalText = button.textContent;
    
    button.textContent = 'TRANSCENDING...';
    button.disabled = true;
    
    // Fake loading animation
    setTimeout(() => {
        button.textContent = 'DESTINATION REACHED';
        setTimeout(() => {
            button.textContent = originalText;
            button.disabled = false;
            console.log('✅ Successfully arrived at nowhere. Welcome!');
        }, 2000);
    }, 3000);
}

function subscribeNewsletter(event) {
    handleNewsletterSubmit(event);
}

function submitContact(event) {
    handleContactSubmit(event);
}

// ========== EASTER EGGS ==========
function initializeEasterEggs() {
    // Konami code easter egg
    let konamiCode = [];
    const konamiSequence = [
        'ArrowUp', 'ArrowUp', 'ArrowDown', 'ArrowDown',
        'ArrowLeft', 'ArrowRight', 'ArrowLeft', 'ArrowRight',
        'KeyB', 'KeyA'
    ];
    
    document.addEventListener('keydown', function(event) {
        konamiCode.push(event.code);
        if (konamiCode.length > konamiSequence.length) {
            konamiCode.shift();
        }
        
        if (konamiCode.join(',') === konamiSequence.join(',')) {
            triggerKonamiEasterEgg();
            konamiCode = [];
        }
    });
    
    // Double-click logo easter egg
    const logo = document.querySelector('.nav-logo img');
    if (logo) {
        logo.addEventListener('dblclick', function() {
            triggerLogoEasterEgg();
        });
    }
    
    // Secret click counter
    let clickCount = 0;
    document.addEventListener('click', function() {
        clickCount++;
        if (clickCount === 42) {
            console.log('%c🎉 You found the meaning of everything!', 'color: #ff8800; font-size: 20px;');
            console.log('%cThe answer was clicking 42 times, just like Douglas Adams predicted.', 'color: #ff8800;');
        }
    });
}

function triggerKonamiEasterEgg() {
    console.log('%c🎮 KONAMI CODE ACTIVATED!', 'color: #ff8800; font-size: 24px; font-weight: bold;');
    console.log('%c🧊 GRIDDB CHEAT MODE ENABLED', 'color: #00ffff; font-size: 18px;');
    
    // Add special effects
    document.body.style.filter = 'hue-rotate(180deg) saturate(150%)';
    document.body.style.animation = 'rainbow 2s linear infinite';
    
    // Create temporary style for rainbow animation
    const style = document.createElement('style');
    style.textContent = `
        @keyframes rainbow {
            0% { filter: hue-rotate(0deg) saturate(150%); }
            100% { filter: hue-rotate(360deg) saturate(150%); }
        }
    `;
    document.head.appendChild(style);
    
    setTimeout(() => {
        document.body.style.filter = '';
        document.body.style.animation = '';
        document.head.removeChild(style);
        console.log('%c✨ Reality restored to normal parameters', 'color: #00ff88;');
    }, 10000);
}

function triggerLogoEasterEgg() {
    console.log('%c🧊 LOGO DOUBLE-CLICK DETECTED!', 'color: #ff00ff; font-size: 16px;');
    console.log('%cActivating enhanced logo rotation protocol...', 'color: #ff00ff;');
    
    const logo = document.querySelector('.nav-logo img');
    logo.style.animation = 'logo-spin 0.5s linear infinite';
    
    setTimeout(() => {
        logo.style.animation = 'logo-spin 8s linear infinite';
        console.log('%c✅ Logo optimization complete', 'color: #00ff88;');
    }, 3000);
}

// ========== UTILITY FUNCTIONS ==========

// Add CSS animations dynamically
function addDynamicStyles() {
    const style = document.createElement('style');
    style.textContent = `
        @keyframes transcendence {
            0% { transform: scale(1) rotate(0deg); filter: hue-rotate(0deg); }
            50% { transform: scale(1.02) rotate(1deg); filter: hue-rotate(180deg); }
            100% { transform: scale(1) rotate(0deg); filter: hue-rotate(360deg); }
        }
        
        .loading .btn-text {
            display: none;
        }
        
        .loading .btn-loading {
            display: inline;
        }
        
        .btn-loading {
            display: none;
        }
    `;
    document.head.appendChild(style);
}

// Initialize dynamic styles
addDynamicStyles();

// Random system messages
setInterval(() => {
    const messages = [
        'System optimization in progress...',
        'Void levels stable at 99.7%',
        'GridDB framework humming efficiently',
        'No bugs detected (suspiciously)',
        'Coffee machine achieving sentience',
        'CEO location still unknown',
        'Productivity levels: Optimized',
        'Data transcendence: 42% complete'
    ];
    
    if (Math.random() < 0.3) { // 30% chance
        const randomMessage = messages[Math.floor(Math.random() * messages.length)];
        console.log(`💫 ${randomMessage}`);
    }
}, 25000);

// Performance monitoring (fake)
setInterval(() => {
    const metrics = {
        voidOptimization: Math.floor(Math.random() * 5 + 95),
        dataTranscendence: Math.floor(Math.random() * 10 + 90),
        absenceEfficiency: Math.floor(Math.random() * 3 + 97),
        realityStability: Math.floor(Math.random() * 20 + 80)
    };
    
    if (Math.random() < 0.1) { // 10% chance
        console.log('%cPERFORMANCE METRICS:', 'color: #00ffff; font-weight: bold;');
        console.table(metrics);
    }
}, 60000);

console.log('%c🎯 All Null Systems protocols initialized successfully', 'color: #00ff88; font-weight: bold;');
console.log('%cRemember: Absence is not the opposite of presence—it\'s the optimization of it.', 'color: #ff00ff; font-style: italic;');