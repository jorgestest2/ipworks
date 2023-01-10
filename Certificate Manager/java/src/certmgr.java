/*
 * IPWorks 2022 Java Edition- Demo Application
 *
 * Copyright (c) 2023 /n software inc. - All rights reserved. - www.nsoftware.com
 *
 */

import java.io.*;

import ipworks.*;

public class certmgr {
    
    static int i = 0;
    
    public static void main(String[] args) {
    	
    	if(args.length!=2) {
			
			System.out.println("usage: certmgr filename password");
			System.out.println("");
			System.out.println("  filename the path to the file containing certificates and optional private keys");
			System.out.println("  password the password for the certificate store file.If test file is used, set the password to \"test\"");
			System.out.println("\r\nExample: certmgr test.pfx test");
		
		} else {
			
			Certmgr certmgr1= new Certmgr();
	    	
	    	try {
	            	    		
	            certmgr1.addCertmgrEventListener(new DefaultCertmgrEventListener(){
	            	public void certList(CertmgrCertListEvent e) {
	            		i++;
	                    System.out.println(i + ". " + e.certSubject);
	                }	            	
	            });	           
	            certmgr1.setCertStoreType(certmgr1.cstJKSFile); //user java key store (JKS file)
	            certmgr1.setCertStore(args[0]);
	            certmgr1.setCertStorePassword(args[1]); //If test file (found in demo folder) is used, set the password to "test"
	            certmgr1.listStoreCertificates();
	        } catch(Exception ex){
	            System.out.println(ex.getMessage());
	        }
	        System.out.println("Exited.");
		}
    	
    }        
}

class ConsoleDemo {
  private static BufferedReader bf = new BufferedReader(new InputStreamReader(System.in));

  static String input() {
    try {
      return bf.readLine();
    } catch (IOException ioe) {
      return "";
    }
  }
  static char read() {
    return input().charAt(0);
  }

  static String prompt(String label) {
    return prompt(label, ":");
  }
  static String prompt(String label, String punctuation) {
    System.out.print(label + punctuation + " ");
    return input();
  }

  static String prompt(String label, String punctuation, String defaultVal)
  {
	System.out.print(label + " [" + defaultVal + "] " + punctuation + " ");
	String response = input();
	if(response.equals(""))
		return defaultVal;
	else
		return response;
  }

  static char ask(String label) {
    return ask(label, "?");
  }
  static char ask(String label, String punctuation) {
    return ask(label, punctuation, "(y/n)");
  }
  static char ask(String label, String punctuation, String answers) {
    System.out.print(label + punctuation + " " + answers + " ");
    return Character.toLowerCase(read());
  }

  static void displayError(Exception e) {
    System.out.print("Error");
    if (e instanceof IPWorksException) {
      System.out.print(" (" + ((IPWorksException) e).getCode() + ")");
    }
    System.out.println(": " + e.getMessage());
    e.printStackTrace();
  }
}




