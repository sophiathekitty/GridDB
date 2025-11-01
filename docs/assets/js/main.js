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
        <div class="fake-login-prompt" style="
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
            animation: fadeInOut 8s ease-in-out;
            cursor: pointer;
            transition: all 0.3s ease;
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
    
    const promptElement = loginDiv.querySelector('.fake-login-prompt');
    
    // Add hover effect
    promptElement.addEventListener('mouseenter', () => {
        promptElement.style.transform = 'scale(1.05)';
        promptElement.style.boxShadow = '0 0 30px var(--primary-cyan)';
    });
    
    promptElement.addEventListener('mouseleave', () => {
        promptElement.style.transform = 'scale(1)';
        promptElement.style.boxShadow = 'var(--glow-cyan)';
    });
    
    // Add click handler for special effects
    promptElement.addEventListener('click', () => {
        triggerSecureLoginEffects(promptElement);
    });
    
    document.body.appendChild(loginDiv);
    
    setTimeout(() => {
        if (loginDiv.parentNode) {
            loginDiv.parentNode.removeChild(loginDiv);
        }
    }, 8000); // Extended to 8 seconds for more interaction time
    
    console.log('🔐 Secure login event simulated');
}

// ========== SECURE LOGIN SPECIAL EFFECTS ==========
let loginClickCount = 0;

function triggerSecureLoginEffects(promptElement) {
    loginClickCount++;
    console.log(`🔐 Secure login clicked! Count: ${loginClickCount}`);
    
    // Immediate visual feedback
    promptElement.style.animation = 'none';
    promptElement.style.transform = 'scale(0.95)';
    promptElement.style.borderColor = 'var(--primary-pink)';
    promptElement.style.boxShadow = '0 0 20px var(--primary-pink)';
    
    setTimeout(() => {
        promptElement.style.transform = 'scale(1)';
    }, 150);
    
    if (loginClickCount === 1) {
        triggerFirstLoginEffect(promptElement);
    } else if (loginClickCount >= 2 && loginClickCount <= 4) {
        triggerSecondLoginEffect(promptElement);
    } else if (loginClickCount >= 5 && loginClickCount <= 8) {
        triggerThirdLoginEffect(promptElement);
    } else {
        triggerUltimateLoginEffect(promptElement);
    }
}

function triggerFirstLoginEffect(promptElement) {
    promptElement.innerHTML = `
        <div style="margin-bottom: 0.5rem; color: var(--primary-pink);">
            🚨 UNAUTHORIZED ACCESS ATTEMPT
        </div>
        <div style="color: var(--text-secondary); font-size: 0.8rem;">
            Security breach detected<br>
            Initiating countermeasures...
        </div>
    `;
    
    // Create security alert particles
    createSecurityParticles('#ff0080');
    showTrippyMessage("SECURITY PROTOCOL ACTIVATED", '#ff0080');
    console.log('� First login hack attempt detected!');
}

function triggerSecondLoginEffect(promptElement) {
    const messages = [
        { title: "⚡ SYSTEM OVERRIDE DETECTED", status: "Bypassing firewall...<br>Access level: ELEVATED" },
        { title: "🔓 ENCRYPTION BYPASSED", status: "Quantum keys compromised<br>Status: INFILTRATING" },
        { title: "💀 MAINFRAME BREACH", status: "Core systems exposed<br>Alert: MAXIMUM THREAT" }
    ];
    
    const msg = messages[Math.floor(Math.random() * messages.length)];
    
    promptElement.innerHTML = `
        <div style="margin-bottom: 0.5rem; color: var(--primary-yellow);">
            ${msg.title}
        </div>
        <div style="color: var(--text-secondary); font-size: 0.8rem;">
            ${msg.status}
        </div>
    `;
    
    createHackerMatrix();
    showTrippyMessage("FIREWALL COMPROMISED", '#ffff00');
    console.log('⚡ Advanced security breach in progress!');
}

function triggerThirdLoginEffect(promptElement) {
    promptElement.innerHTML = `
        <div style="margin-bottom: 0.5rem; color: var(--primary-cyan);">
            🌐 ROOT ACCESS ACHIEVED
        </div>
        <div style="color: var(--text-secondary); font-size: 0.8rem;">
            Welcome, Admin<br>
            System: FULLY COMPROMISED
        </div>
    `;
    
    createRootAccessGlitch();
    createFloatingBinaryCode();
    showTrippyMessage("WELCOME TO THE MAINFRAME", '#00ffff');
    console.log('🌐 Root access granted! System compromised!');
}

function triggerUltimateLoginEffect(promptElement) {
    promptElement.innerHTML = `
        <div style="margin-bottom: 0.5rem; color: var(--primary-green);">
            👑 GOD MODE ACTIVATED
        </div>
        <div style="color: var(--text-secondary); font-size: 0.8rem;">
            Status: REALITY ADMIN<br>
            Permissions: UNLIMITED
        </div>
    `;
    
    // All effects at once
    createSecurityParticles('#00ff88');
    createHackerMatrix();
    createRootAccessGlitch();
    createFloatingBinaryCode();
    createRealityGlitch();
    showTrippyMessage("YOU ARE NOW THE SYSTEM", '#00ff88');
    console.log('👑 ULTIMATE HACKER STATUS ACHIEVED!');
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

// ========== ENHANCED EXPERIENCE THE FUTURE SYSTEM ==========
let experienceCount = 0;
let unlockedEffects = new Set();

function goNowhere() {
    console.log('🚀 Initiating journey to nowhere...');
    experienceCount++;
    
    const button = event.target;
    const originalText = button.textContent;
    
    button.textContent = 'TRANSCENDING...';
    button.disabled = true;
    
    // Determine which effect to trigger based on click count
    if (experienceCount === 1) {
        triggerFirstTimeEffect(button, originalText);
    } else if (experienceCount >= 3 && experienceCount <= 7 && !unlockedEffects.has('level2')) {
        if (Math.random() < 0.4 || experienceCount >= 6) { // 40% chance, forced at 6 clicks
            triggerSecondLevelEffect(button, originalText);
            unlockedEffects.add('level2');
        } else {
            triggerNormalEffect(button, originalText);
        }
    } else if (experienceCount >= 8 && experienceCount <= 15 && !unlockedEffects.has('level3')) {
        if (Math.random() < 0.3 || experienceCount >= 12) { // 30% chance, forced at 12 clicks
            triggerThirdLevelEffect(button, originalText);
            unlockedEffects.add('level3');
        } else {
            triggerRandomSpecialEffect(button, originalText);
        }
    } else if (experienceCount >= 16) {
        triggerUltimateEffect(button, originalText);
    } else {
        triggerRandomSpecialEffect(button, originalText);
    }
}

function triggerFirstTimeEffect(button, originalText) {
    console.log('✨ FIRST TRANSCENDENCE DETECTED - UNLOCKING REALITY MATRIX');
    
    // Create the first progress cube
    createProgressCube(1, '#00ffff');
    
    const messages = [
        "REALITY MATRIX UNLOCKED",
        "WELCOME TO THE VOID DIMENSION",
        "YOUR CONSCIOUSNESS HAS BEEN UPGRADED"
    ];
    
    showTrippyMessage(messages[Math.floor(Math.random() * messages.length)], '#00ffff');
    
    setTimeout(() => {
        button.textContent = 'REALITY ENHANCED';
        setTimeout(() => {
            button.textContent = originalText;
            button.disabled = false;
            console.log('🌟 Welcome to enhanced reality. The cube watches over you now.');
        }, 2000);
    }, 3000);
}

function triggerSecondLevelEffect(button, originalText) {
    console.log('🔥 SECOND LEVEL TRANSCENDENCE - QUANTUM EXPLOSION ACTIVATED');
    
    // Create explosion effect and second progress cube
    createExplosionEffect();
    createProgressCube(2, '#ff00ff');
    
    const messages = [
        "QUANTUM BARRIERS SHATTERED",
        "CONSCIOUSNESS OVERFLOW DETECTED",
        "YOU HAVE ACHIEVED DIMENSIONAL PHASE-SHIFT",
        "THE VOID RECOGNIZES YOUR DEDICATION",
        "REALITY.EXE HAS STOPPED RESPONDING"
    ];
    
    showTrippyMessage(messages[Math.floor(Math.random() * messages.length)], '#ff00ff');
    
    setTimeout(() => {
        button.textContent = 'QUANTUM ACHIEVED';
        setTimeout(() => {
            button.textContent = originalText;
            button.disabled = false;
            console.log('💥 Quantum explosion successful. Reality has been optimized.');
        }, 2500);
    }, 4000);
}

function triggerThirdLevelEffect(button, originalText) {
    console.log('🌌 THIRD LEVEL TRANSCENDENCE - COSMIC ENLIGHTENMENT INITIATED');
    
    // Create multiple effects and third progress cube
    createCosmicRippleEffect();
    createProgressCube(3, '#ffff00');
    createFloatingSymbols();
    
    const messages = [
        "COSMIC ENLIGHTENMENT ACHIEVED",
        "YOU HAVE TRANSCENDED THE NEED FOR EXISTENCE",
        "THE UNIVERSE IS NOW YOUR SCREENSAVER",
        "CONGRATULATIONS: YOU NO LONGER EXIST (THIS IS OPTIMAL)",
        "VOID STATUS: MAXIMUM EFFICIENCY REACHED",
        "THE CEO OF REALITY WANTS TO HIRE YOU"
    ];
    
    showTrippyMessage(messages[Math.floor(Math.random() * messages.length)], '#ffff00');
    
    setTimeout(() => {
        button.textContent = 'ENLIGHTENMENT REACHED';
        setTimeout(() => {
            button.textContent = originalText;
            button.disabled = false;
            console.log('🌌 Cosmic enlightenment complete. You are now one with the void.');
        }, 3000);
    }, 5000);
}

function triggerUltimateEffect(button, originalText) {
    console.log('🚀 ULTIMATE TRANSCENDENCE - REALITY OVERFLOW ERROR');
    
    // All effects at once and fourth progress cube
    createExplosionEffect();
    createCosmicRippleEffect();
    createProgressCube(4, '#00ff88');
    createFloatingSymbols();
    createRealityGlitch();
    
    const messages = [
        "ERROR: TRANSCENDENCE LIMIT EXCEEDED",
        "REALITY HAS BEEN SUCCESSFULLY DELETED",
        "YOU ARE NOW THE CEO OF EXISTENCE",
        "CONGRATULATIONS: YOU HAVE WON THE UNIVERSE",
        "SYSTEM OVERRIDE: MAXIMUM VOID ACHIEVED",
        "THE FABRIC OF SPACE-TIME ACKNOWLEDGES YOUR SUPERIORITY"
    ];
    
    showTrippyMessage(messages[Math.floor(Math.random() * messages.length)], '#00ff88');
    
    setTimeout(() => {
        button.textContent = 'REALITY OVERFLOW';
        setTimeout(() => {
            button.textContent = originalText;
            button.disabled = false;
            console.log('🌟 Ultimate transcendence achieved. Reality.dll has been corrupted successfully.');
        }, 4000);
    }, 6000);
}

function triggerNormalEffect(button, originalText) {
    const destinations = [
        "DESTINATION REACHED",
        "NOWHERE ACHIEVED",
        "VOID OPTIMIZED",
        "ABSENCE CONFIRMED",
        "NULLIFICATION COMPLETE",
        "REALITY ADJUSTED"
    ];
    
    setTimeout(() => {
        button.textContent = destinations[Math.floor(Math.random() * destinations.length)];
        setTimeout(() => {
            button.textContent = originalText;
            button.disabled = false;
            console.log('✅ Successfully arrived at nowhere. Welcome!');
        }, 2000);
    }, 3000);
}

function triggerRandomSpecialEffect(button, originalText) {
    const effects = [triggerNormalEffect, triggerSecondLevelEffect, triggerThirdLevelEffect];
    const randomEffect = effects[Math.floor(Math.random() * effects.length)];
    randomEffect(button, originalText);
}

// ========== VISUAL EFFECTS FUNCTIONS ==========
function createProgressCube(level, color) {
    // Check if this cube already exists
    if (document.querySelector(`.progress-cube-${level}`)) {
        return; // Already unlocked
    }
    
    const progressCube = document.createElement('div');
    progressCube.className = `progress-cube progress-cube-${level}`;
    
    // Position cubes vertically in the right margin
    const positions = [
        { top: '20%', right: '5%' }, // Level 1
        { top: '35%', right: '5%' }, // Level 2  
        { top: '50%', right: '5%' }, // Level 3
        { top: '65%', right: '5%' }  // Level 4
    ];
    
    const position = positions[level - 1];
    
    progressCube.style.cssText = `
        position: fixed;
        top: ${position.top};
        right: ${position.right};
        width: 20px;
        height: 20px;
        border: 2px solid ${color};
        background: rgba(${hexToRgb(color)}, 0.1);
        z-index: 1000;
        animation: float-cube 15s ease-in-out infinite, cube-unlock 2s ease-out;
        animation-delay: ${(level - 1) * 2}s, 0s;
        opacity: 0;
        animation-fill-mode: forwards;
        box-shadow: 0 0 10px ${color};
    `;
    
    document.body.appendChild(progressCube);
    console.log(`🔓 Progress cube ${level} unlocked with color ${color}`);
}

function hexToRgb(hex) {
    const result = /^#?([a-f\d]{2})([a-f\d]{2})([a-f\d]{2})$/i.exec(hex);
    if (result) {
        return `${parseInt(result[1], 16)}, ${parseInt(result[2], 16)}, ${parseInt(result[3], 16)}`;
    }
    // Fallback for 3-char hex or named colors
    if (hex === '#00ffff') return '0, 255, 255';
    if (hex === '#ff00ff') return '255, 0, 255';
    if (hex === '#ffff00') return '255, 255, 0';
    if (hex === '#00ff88') return '0, 255, 136';
    return '0, 255, 255'; // Default cyan
}

function createExplosionEffect() {
    for (let i = 0; i < 20; i++) {
        const particle = document.createElement('div');
        particle.style.cssText = `
            position: fixed;
            top: 50%;
            left: 50%;
            width: 4px;
            height: 4px;
            background: ${i % 2 ? '#ff00ff' : '#00ffff'};
            border-radius: 50%;
            pointer-events: none;
            z-index: 1001;
            animation: explode-${i} 2s ease-out forwards;
        `;
        
        // Create unique explosion animation for each particle
        const style = document.createElement('style');
        style.textContent = `
            @keyframes explode-${i} {
                0% { transform: translate(-50%, -50%) scale(1); opacity: 1; }
                100% { 
                    transform: translate(
                        ${(Math.random() - 0.5) * 400}px,
                        ${(Math.random() - 0.5) * 400}px
                    ) scale(0);
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
        document.body.appendChild(particle);
        
        setTimeout(() => {
            if (particle.parentNode) particle.parentNode.removeChild(particle);
            if (style.parentNode) style.parentNode.removeChild(style);
        }, 2000);
    }
}

function createCosmicRippleEffect() {
    for (let i = 0; i < 5; i++) {
        const ripple = document.createElement('div');
        ripple.style.cssText = `
            position: fixed;
            top: 50%;
            left: 50%;
            width: 10px;
            height: 10px;
            border: 2px solid #ffff00;
            border-radius: 50%;
            pointer-events: none;
            z-index: 999;
            animation: cosmic-ripple ${2 + i * 0.5}s ease-out forwards;
            animation-delay: ${i * 0.3}s;
        `;
        
        document.body.appendChild(ripple);
        
        setTimeout(() => {
            if (ripple.parentNode) ripple.parentNode.removeChild(ripple);
        }, 5000);
    }
}

function createFloatingSymbols() {
    const symbols = ['∅', '⚡', '🧊', '∞', '◊', '△', '▽', '◈'];
    for (let i = 0; i < 8; i++) {
        const symbol = document.createElement('div');
        symbol.textContent = symbols[i];
        symbol.style.cssText = `
            position: fixed;
            top: ${Math.random() * 80 + 10}%;
            left: ${Math.random() * 80 + 10}%;
            font-size: 24px;
            color: #00ff88;
            pointer-events: none;
            z-index: 1000;
            animation: float-symbol 4s ease-in-out forwards;
            text-shadow: 0 0 10px #00ff88;
        `;
        
        document.body.appendChild(symbol);
        
        setTimeout(() => {
            if (symbol.parentNode) symbol.parentNode.removeChild(symbol);
        }, 4000);
    }
}

function createRealityGlitch() {
    const glitchOverlay = document.createElement('div');
    glitchOverlay.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: linear-gradient(45deg, transparent, rgba(255,0,255,0.1), transparent);
        pointer-events: none;
        z-index: 1002;
        animation: reality-glitch 3s ease-in-out;
    `;
    
    document.body.appendChild(glitchOverlay);
    
    setTimeout(() => {
        if (glitchOverlay.parentNode) glitchOverlay.parentNode.removeChild(glitchOverlay);
    }, 3000);
}

function showTrippyMessage(message, color) {
    const messageDiv = document.createElement('div');
    messageDiv.textContent = message;
    messageDiv.style.cssText = `
        position: fixed;
        top: 30%;
        left: 50%;
        transform: translateX(-50%);
        font-size: 24px;
        font-weight: bold;
        color: ${color};
        text-shadow: 0 0 20px ${color};
        text-align: center;
        pointer-events: none;
        z-index: 1003;
        animation: trippy-message 4s ease-in-out forwards;
        font-family: var(--font-primary);
    `;
    
    document.body.appendChild(messageDiv);
    
    setTimeout(() => {
        if (messageDiv.parentNode) messageDiv.parentNode.removeChild(messageDiv);
    }, 4000);
}

// ========== LOGIN-SPECIFIC VISUAL EFFECTS ==========
function createSecurityParticles(color) {
    for (let i = 0; i < 15; i++) {
        const particle = document.createElement('div');
        particle.textContent = '🔐';
        particle.style.cssText = `
            position: fixed;
            top: 20px;
            right: 100px;
            font-size: 16px;
            color: ${color};
            pointer-events: none;
            z-index: 1002;
            animation: security-scatter-${i} 3s ease-out forwards;
        `;
        
        // Create unique scatter animation
        const style = document.createElement('style');
        style.textContent = `
            @keyframes security-scatter-${i} {
                0% { transform: translate(0, 0) rotate(0deg); opacity: 1; }
                100% { 
                    transform: translate(
                        ${(Math.random() - 0.5) * 300}px,
                        ${(Math.random() - 0.5) * 300}px
                    ) rotate(${Math.random() * 360}deg);
                    opacity: 0;
                }
            }
        `;
        document.head.appendChild(style);
        document.body.appendChild(particle);
        
        setTimeout(() => {
            if (particle.parentNode) particle.parentNode.removeChild(particle);
            if (style.parentNode) style.parentNode.removeChild(style);
        }, 3000);
    }
}

function createHackerMatrix() {
    for (let i = 0; i < 20; i++) {
        const code = document.createElement('div');
        const binaryString = Math.random().toString(2).substr(2, 8);
        code.textContent = binaryString;
        code.style.cssText = `
            position: fixed;
            top: ${Math.random() * 100}%;
            left: ${Math.random() * 100}%;
            font-size: 12px;
            color: #00ff00;
            font-family: monospace;
            pointer-events: none;
            z-index: 1001;
            animation: matrix-fall 4s linear forwards;
            opacity: 0.7;
        `;
        
        document.body.appendChild(code);
        
        setTimeout(() => {
            if (code.parentNode) code.parentNode.removeChild(code);
        }, 4000);
    }
}

function createRootAccessGlitch() {
    const glitch = document.createElement('div');
    glitch.style.cssText = `
        position: fixed;
        top: 0;
        left: 0;
        width: 100%;
        height: 100%;
        background: linear-gradient(90deg, 
            transparent 0%, 
            rgba(0, 255, 255, 0.1) 25%, 
            transparent 50%,
            rgba(0, 255, 255, 0.1) 75%,
            transparent 100%);
        pointer-events: none;
        z-index: 999;
        animation: root-access-scan 2s ease-in-out;
    `;
    
    document.body.appendChild(glitch);
    
    setTimeout(() => {
        if (glitch.parentNode) glitch.parentNode.removeChild(glitch);
    }, 2000);
}

function createFloatingBinaryCode() {
    const codes = ['01001000', '01000001', '01000011', '01001011', '01000101', '01010010'];
    for (let i = 0; i < codes.length; i++) {
        const binary = document.createElement('div');
        binary.textContent = codes[i];
        binary.style.cssText = `
            position: fixed;
            top: ${20 + i * 10}%;
            right: ${10 + Math.random() * 20}%;
            font-size: 14px;
            color: #00ffff;
            font-family: monospace;
            pointer-events: none;
            z-index: 1001;
            animation: float-binary 6s ease-in-out forwards;
            text-shadow: 0 0 10px #00ffff;
        `;
        
        document.body.appendChild(binary);
        
        setTimeout(() => {
            if (binary.parentNode) binary.parentNode.removeChild(binary);
        }, 6000);
    }
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