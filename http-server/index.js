import * as forge from "node-forge";
import tls from "node:tls";
import http2 from "node:http2";
import { hash } from "node:crypto";

const port = 8123;

const SNICallback = (name, cb) => {
    try {
        console.log(`SNICallback: ${name}`);
        const { key, cert } = createSelfSignedCert();
        cb(null, tls.createSecureContext({ key, cert }));
    } catch (error) {
        console.error(error);
    }
};

const httpServer = http2.createSecureServer({
    SNICallback,
    allowHTTP1: true,
    keepAlive: false
}, (req, res) => {
    try {
        console.log(`${req.httpVersion} Authority: ${req.authority ?? req.headers.host}`);
        res.end("ok");
    } catch (error) {
        console.error(error);
    }
});

httpServer.listen(port, () => {
    console.log(`Server started on port ${port}`)
});

httpServer.on("secureConnection", (s) => {
    console.log(`New TLS Socket ${s.servername} ${hash("md5",s.getSession()).toString("hex")}`);
})

function createSelfSignedCert() {

    let key;
    let cert;


    const pki = forge.default.pki;

    // generate a key pair or use one you have already
    const keys = pki.rsa.generateKeyPair(2048);

    // create a new certificate
    const crt = pki.createCertificate();

    // fill the required fields
    crt.publicKey = keys.publicKey;
    crt.serialNumber = '01';
    crt.validity.notBefore = new Date();
    crt.validity.notAfter = new Date();
    crt.validity.notAfter.setFullYear(crt.validity.notBefore.getFullYear() + 40);

    // use your own attributes here, or supply a csr (check the docs)
    const attrs = [
        {
            name: 'commonName',
            value: 'dev.socialmail.in'
        }, {
            name: 'countryName',
            value: 'IN'
        }, {
            shortName: 'ST',
            value: 'Maharashtra'
        }, {
            name: 'localityName',
            value: 'Navi Mumbai'
        }, {
            name: 'organizationName',
            value: 'NeuroSpeech Technologies Pvt Ltd'
        }, {
            shortName: 'OU',
            value: 'Test'
        }
    ];

    // here we set subject and issuer as the same one
    crt.setSubject(attrs);
    crt.setIssuer(attrs);

    // the actual certificate signing
    crt.sign(keys.privateKey);

    // now convert the Forge certificate to PEM format
    cert = pki.certificateToPem(crt);
    key = pki.privateKeyToPem(keys.privateKey);

    return { key, cert };

}